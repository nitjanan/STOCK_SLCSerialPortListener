using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SerialPortListener.Serial;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Messaging;
using Devart.Data.PostgreSql;
using static SerialPortListener.TableFromDB;
using System.Data.Odbc;
using Microsoft.VisualBasic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;


namespace SerialPortListener
{

    public partial class MainForm : Form
    {
        SerialPortManager _spManager;
        Datalayer dl;
        String strCalQ = "1.00";
        AutoCompleteStringCollection collCarTeam = new AutoCompleteStringCollection();
        bool isCheckedCash = false;
        bool isCheckedTrans = false;
        bool isCheckedCredit = false;
        bool isCheckedMill1 = false;
        bool isCheckedMill2 = false;
        bool isCheckedMill3 = false;
        bool isCheckedMillNo = false;
        bool isCheckedCleanStone = false;
        bool isCheckedCleanWater = false;
        bool isCheckedCleanNo = false;
        bool isCheckedSelfPick = false;
        bool isCheckedSendTo = false;

        class ComboboxValue
        {
            public string Id { get; private set; }
            public string Name { get; private set; }

            public ComboboxValue(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public MainForm(string username, String firstname)
        {
            dl = new Datalayer();
            InitializeComponent();

            UserInitialization();

            setDefaultFromDB(username, firstname);

            getSettingDefault();

            // Default COM port reader to stop state on program launch
            ucBackup.CheckUpdateRequested += BtnCheckUpdate_Click;

            // เช็คอัพเดทแบบเงียบตอนเปิดโปรแกรม — ถ้าเชื่อมต่อ Server ไม่ได้ต้องไม่ทำให้ฟอร์มเปิดไม่ขึ้น
            // ใช้ BeginInvoke ให้รันหลังจากฟอร์มแสดงผลเสร็จแล้ว และไม่ await ใน constructor (fire-and-forget)
            this.Load += async (s, e) => await CheckForUpdateAsync(silent: true);

            // _spManager.StartListening();
        }

        // ปุ่ม "ตรวจสอบอัพเดท" อยู่ที่ ucBackup ; MainForm รับ event มาทำงานเพราะ logic ต้องใช้ dl, findBWS(), GetJwtToken()
        private async void BtnCheckUpdate_Click(object sender, EventArgs e)
        {
            ucBackup.CheckUpdateButtonEnabled = false;
            try
            {
                await CheckForUpdateAsync(silent: false);
            }
            finally
            {
                ucBackup.CheckUpdateButtonEnabled = true;
            }
        }

        // silent = true: เรียกตอนเปิดโปรแกรม — ถ้าต่อ Server ไม่ได้หรือเป็นเวอร์ชันล่าสุดอยู่แล้วจะไม่ขึ้น MessageBox กวนใจ
        // silent = false: เรียกจากปุ่ม "ตรวจสอบอัพเดท" — แจ้งผลทุกกรณี
        // ทุก exception ถูกดักไว้ในนี้ทั้งหมด เพื่อไม่ให้ปัญหาการเช็คอัพเดทกระทบการเปิดฟอร์มหลักของโปรแกรม
        private async Task CheckForUpdateAsync(bool silent)
        {
            try
            {
                string baseUrl = getBaseApi(1, 1);
                string apiUsername = getBaseApi(2, 1);
                string apiPassword = getBaseApi(3, 1);

                using (HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) })
                {
                    string accessToken;
                    try
                    {
                        accessToken = await GetJwtToken(client, baseUrl, apiUsername, apiPassword);
                    }
                    catch (Exception)
                    {
                        accessToken = null;
                    }

                    if (accessToken == null)
                    {
                        if (!silent)
                            MessageBox.Show("ไม่สามารถเชื่อมต่อ Server เพื่อเช็คอัพเดทได้", "เช็คอัพเดท",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    AppReleaseInfo release = await AppUpdateService.GetLatestReleaseAsync(client, baseUrl, accessToken);
                    Version currentVersion = AppUpdateService.CurrentVersion;

                    if (release == null || !AppUpdateService.IsNewerVersion(release.version, currentVersion))
                    {
                        if (!silent)
                            MessageBox.Show($"คุณใช้เวอร์ชันล่าสุดแล้ว ({currentVersion})", "เช็คอัพเดท",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    string message = $"พบเวอร์ชันใหม่ {release.version}\r\n\r\n{release.release_notes}\r\n\r\nต้องการดาวน์โหลดและติดตั้งตอนนี้หรือไม่?";
                    DialogResult confirm = MessageBox.Show(message, "พบอัพเดทใหม่",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm != DialogResult.Yes)
                        return;

                    string installerPath = await AppUpdateService.DownloadInstallerAsync(client, release, baseUrl);

                    bool sqlApplied = false;
                    if (!string.IsNullOrEmpty(release.sql_script_url))
                    {
                        dl.connect();
                        try
                        {
                            sqlApplied = await AppUpdateService.DownloadAndRunSqlScriptAsync(client, release, baseUrl, dl.sqlConn());
                        }
                        finally
                        {
                            dl.close();
                        }
                    }

                    await AppUpdateService.LogUpdateAsync(
                        client, baseUrl, accessToken, Environment.MachineName,
                        currentVersion.ToString(), release.version, true, findBWS(), sqlApplied);

                    AppUpdateService.RunInstallerAndExit(installerPath);
                }
            }
            catch (Exception ex)
            {
                if (!silent)
                    MessageBox.Show("เกิดข้อผิดพลาดระหว่างเช็ค/ติดตั้งอัพเดท: " + ex.Message, "เช็คอัพเดท",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<string> GetJwtToken(HttpClient client, string baseUrl, string username, string password)
        {
            try
            {
                string jwtUrl = $"{baseUrl}/jwt/create/";

                var loginData = new
                {
                    username = username,
                    password = password
                };

                string loginJson =
                    JsonConvert.SerializeObject(loginData);

                var loginContent =
                    new StringContent(
                        loginJson,
                        Encoding.UTF8,
                        "application/json"
                    );

                HttpResponseMessage jwtResponse =
                    await client.PostAsync(jwtUrl, loginContent);

                if (!jwtResponse.IsSuccessStatusCode)
                {
                    string jwtError =
                        await jwtResponse.Content.ReadAsStringAsync();

                    return null;
                }

                string jwtResult =
                    await jwtResponse.Content.ReadAsStringAsync();

                dynamic jwtObj =
                    JsonConvert.DeserializeObject(jwtResult);

                return jwtObj.access.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string findBWS()
        {
            string code = "";
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT code FROM base_weight_station WHERE base_weight_station_id = 1";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    code = reader["code"].ToString();
                }
            }
            catch (Exception)
            {

            }
            dl.close();

            return code;
        }

        private string getBaseApi(int mode, int base_api_id)
        {
            string url = "";
            string username = "";
            string password = "";
            string comp_code = "";
            string token = "";

            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT url, username, password, comp_code, token FROM base_api where id = " + base_api_id;
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    url = reader["url"].ToString();
                    username = reader["username"].ToString();
                    password = reader["password"].ToString();
                    comp_code = reader["comp_code"].ToString();
                    token = reader["token"].ToString();
                }
            }
            catch (Exception)
            {

            }
            dl.close();

            if (mode.Equals(1))
                return url;
            else if (mode.Equals(2))
                return username;
            else if (mode.Equals(3))
                return password;
            else if (mode.Equals(4))
                return comp_code;
            else if (mode.Equals(5))
                return token;
            else
                return "";
        }

        public void getSettingDefault()
        {
            lbCompanyCode.Text = Company.Code;

            /* autoComplete ผู้ตัก */
            autoCompleteSettingByCompany(tbScoopId, "รหัสผู้ตัก", "base_scoop");
            autoCompleteSettingByCompany(tbScoopName, "ชื่อผู้ตัก", "base_scoop");

            /* autoComplete ผู้ชั่ง */
            autoCompleteSetting(tbScaleId, "username", "users");
            autoCompleteSetting(tbScaleName, "firstname", "users");

            /* autoComplete ผู้อนุมัติ */
            autoCompleteSetting(tbApproveId, "รหัสผู้อนุมัติจ่าย", "base_approve");
            autoCompleteSetting(tbApproveName, "ชื่อผู้อนุมัติจ่าย", "base_approve");

            /* autoComplete จังหวัด */
            autoCompleteSetting(tbCarCity, "ชื่อจังหวัด", "base_car_city");

            /* autoComplete ลูกค้า */
            autoCompleteSettingByWeightType(tbCustomerId, "รหัสลูกค้า", "base_customer");
            autoCompleteSettingByWeightType(tbCustomerName, "ชื่อลูกค้า", "base_customer");

            /* autoComplete ผู้ขับ */
            autoCompleteSettingByCompany(tbDriverId, "รหัสผู้ขับ", "base_driver");
            autoCompleteSettingByCompany(tbDriverName, "ชื่อผู้ขับ", "base_driver");

            autoCompleteSettingByWeightType(tbMillId, "รหัสโรงโม่", "base_mill");
            autoCompleteSettingByWeightType(tbMillName, "ชื่อโรงโม่", "base_mill");

            autoCompleteSettingByWeightType(tbSiteId, "base_site_id", "base_site");
            autoCompleteSettingByWeightType(tbSiteName, "base_site_name", "base_site");

            autoCompleteSettingInactive(tbStoneTypeId, "รหัสหิน", "base_stone_type");
            autoCompleteSettingInactive(tbStoneTypeName, "ชื่อหิน", "base_stone_type");

            /* autoComplete ทะเบียนรถ */
            autoCompleteSettingByCompany(tbCarLicenseId, "รหัสทะเบียนรถ", "base_car_registration");
            autoCompleteSettingByCompany(tbCarLicense, "ชื่อทะเบียนรถ", "base_car_registration");

            Weight.CustomerAddress = getPrintFromDB("base_customer", "ที่อยู่", "รหัสลูกค้า", tbCustomerId.Text);

            tbWeigtData.Enter += (s, e) => { tbWeigtData.Parent.Focus(); };

        }
        public void EnableWeightInAndOut()
        {
            btReadIn.Enabled = true;
            btReadOut.Enabled = true;
        }

        public void disableReadWeightIn() {
            btReadIn.Enabled = false;
        }

        public void disableReadWeightOut() {
            btReadOut.Enabled = false;
        }

        public void resetMainForm() {
            tbId.Text = "";
            tbDocNum.Text = "";
            rbMill1.Checked = false;
            rbMill2.Checked = false;
            rbMill3.Checked = false;
            rbMillNo.Checked = false;
            rbCash.Checked = false;
            rbCredit.Checked = false;
            rbTrans.Checked = false;
            rbVat.Checked = false;
            cbbStoneType.Text = "";
            cbbStoneColor.Text = "";
            cbbTransport.Text = "";
            tbRefNum.Text = "";
            tbCustomerId.Text = "";
            tbCustomerName.Text = "";
            tbCarLicense.Text = "";
            tbCarCity.Text = "";
            tbDriverName.Text = "";
            tbCarLicenseId.Text = "";
            tbDriverId.Text = "";

            tbStoneTypeId.Text = "";
            tbStoneTypeName.Text = "";
            tbMillId.Text = "";
            tbMillName.Text = "";
            tbSiteId.Text = "";
            tbSiteName.Text = "";

            tbScaleId.Text = Globals.Username;
            tbScaleName.Text = Globals.Firstname;
            
            tbScoopId.Text = "";
            tbScoopName.Text = "";
            tbWeightIn.Text = "0.00";
            tbWeightOut.Text = "0.00";
            tbWeightTotal.Text = "0.00";
            tbPricePerTon.Text = "0.00";
            tbAmountVat.Text = "0.00";
            tbAmount.Text = "0.00";
            tbShipCost.Text = "0.00";
            tbAmount.Text = "0.00";
            tbVat.Text = "0.00";
            tbApproveId.Text = "";
            tbApproveName.Text = "";
            dtDate.Text = DateTime.Now.ToShortDateString();
            dtWeightInDate.Text = DateTime.Now.ToShortDateString();
            dtWeightOutDate.Text = DateTime.Now.ToShortDateString();
            dtWeightInTime.Text = DateTime.Now.ToShortTimeString();
            dtWeightOutTime.Text = DateTime.Now.ToShortTimeString();
            tbQ.Text = "0.00";
            rbbNonVat.Checked = false;
            rbbVat.Checked = false;
            rbCleanStone.Checked = false;
            rbCleanWater.Checked = false;
            rbCleanNo.Checked = false;
            tbSite.Text = "";
            tbCarTeam.Text = "";
            cbbMill.Text = "";
            cbbSite.Text = "";
            tbNote.Text = "";
            fillStoneCombo();
            fillMillCombo();
            fillSiteCombo();
            calculatenumQ();

            disableBtAfterRead(0);

            //if user admin enable all 
            if (Globals.isPermissionEditWeight())
                disableBtAfterRead(999);
        }

        public void getAndSetFirstUser()
        {

            //เพิ่ม combobox
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM users ORDER BY users_id asc LIMIT 1";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    tbScaleId.Text = reader["username"].ToString();
                    tbScaleName.Text = reader["firstname"].ToString();
                }
            }
            catch (Exception)
            {

            }
            dl.close();
        }

        public void runningDocNumber() {
            Boolean IsnewYear = false;
            string todayYear = DateTime.Now.ToString("yyyy");

            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public.seq_doc_num where run_year = '"+ todayYear + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                if (reader.Read())
                {
                    int rdNum = Convert.ToInt32(reader["run_number"].ToString());
                    rdNum++;
                    int lengthRdNum = reader["run_number"].ToString().Length;
                    string format = "D" + lengthRdNum.ToString();
                    tbDocNum.Text = rdNum.ToString(format);
                }
                else
                {
                    IsnewYear = true;
                }
            }
            catch (Exception)
            {
            }
            dl.close();

            if (IsnewYear)
                generateNewSeqNumber();

        }

        private void generateNewSeqNumber() {
            string todayYear = DateTime.Now.ToString("yyyy");
            string runningNumber = "000000";
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "INSERT INTO public.seq_doc_num (run_number, run_year) " +
                    "VALUES ('" + runningNumber + "', '" + todayYear + "') ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
            }
            catch (Exception) { 
            }
            dl.close();

            //แก้ใน form
            int rdNumNew = Convert.ToInt32(runningNumber);
            rdNumNew++;
            int lengthRdNum = runningNumber.ToString().Length;
            string format = "D" + lengthRdNum.ToString();
            tbDocNum.Text = rdNumNew.ToString(format);
        }

        public void checkDocNumEmty()
        {
            if (tbDocNum.Text == "")
            {
                tbDocNum.Enabled = true;
            }
        }

        private void fillStoneCombo() {
            //ล้างก่อน
            cbbStoneType.Items.Clear();
            //เพิ่ม combobox
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public.base_stone_type where inactive = false ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read()) {
                    string id = reader["รหัสหิน"].ToString();
                    string des = reader["ชื่อหิน"].ToString();
                    cbbStoneType.Items.Add(new ComboboxValue(id, des));
                }
            }
            catch (Exception) {

            }
            dl.close();
        }

        private void fillMillCombo()
        {
            //ล้างก่อน
            cbbMill.Items.Clear();
            //เพิ่ม combobox
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public.base_mill where weight_type = 2 or weight_type = 3";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader["รหัสโรงโม่"].ToString();
                    string des = reader["ชื่อโรงโม่"].ToString();
                    cbbMill.Items.Add(new ComboboxValue(id, des));
                }
            }
            catch (Exception)
            {

            }
            dl.close();
        }

        private void fillSiteCombo()
        {
            //ล้างก่อน
            cbbSite.Items.Clear();
            //เพิ่ม combobox
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT base_site_id, base_site_name FROM public.base_site where weight_type = 2 or weight_type = 3";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader["base_site_id"].ToString();
                    string des = reader["base_site_name"].ToString();
                    cbbSite.Items.Add(new ComboboxValue(id, des));
                }
            }
            catch (Exception)
            {

            }
            dl.close();
        }

        //เรียกจาก TableFromDB
        public void AfterGetDataFromTable() {
            ucTruck.Hide();
            ucReport.Hide();
            ucHelp.Hide();
            ucSetting.Hide();
        }

        public void setDataFromClassTableFromDB(DataToUpdate data) {
            tbId.Text = data.id;
            dtDate.Text = data.date;
            tbDocNum.Text = data.docNum;
            tbCarLicense.Text = data.carLicense;
            tbCarCity.Text = data.carCity;
            tbDriverName.Text = data.driverName;
            tbCustomerId.Text = data.customerId;
            tbCustomerName.Text = data.customerName;
            tbWeightIn.Text = tonTokg(data.weightIn);
            tbWeightOut.Text = tonTokg(data.weightOut);
            tbWeightTotal.Text = tonTokg(data.weightTotal);
            tbRefNum.Text = data.refNum;
            tbScaleId.Text = data.scaleId;
            tbScaleName.Text = data.scaleName;
            tbScoopId.Text = data.scoopId;
            tbScoopName.Text = data.scoopName;
            tbPricePerTon.Text = numberFormat(data.pricePerTon, 2);
            tbAmountVat.Text = numberFormat(data.amountVat, 2);
            tbAmount.Text = numberFormat(data.amount, 2);
            tbShipCost.Text = data.shipCost;
            dtWeightInDate.Text = data.weightInDate;
            dtWeightInTime.Text = data.weightInTime;
            if (tbWeightOut.Text == "0.00")
            {
                dtWeightOutDate.Text = DateTime.Now.ToShortDateString();
                dtWeightOutTime.Text = DateTime.Now.ToShortTimeString();
                btReadOut.Enabled = true;
            }
            else {
                dtWeightOutDate.Text = data.weightOutDate;
                dtWeightOutTime.Text = data.weightOutTime;
                //disable after read out
                disableBtAfterRead(2);
            }
            tbStoneTypeId.Text = data.stoneTypeId;
            tbStoneTypeName.Text = data.stoneType;

            tbQ.Text = numberFormat(data.q,2);
            tbApproveId.Text = data.approveId;
            tbApproveName.Text = data.approveName;
            cbbStoneColor.Text = data.stoneColor;
            cbbTransport.Text = data.transport;
            tbCarTeam.Text = data.team;
            tbCarLicenseId.Text = data.carLicenseId;
            tbDriverId.Text = data.driverId;

            tbSiteId.Text = data.siteId;//111111111111
            tbSiteName.Text = data.site;//111111111111

            tbMillId.Text = data.millId;//111111111111
            tbMillName.Text = data.mill;//111111111111
            tbNote.Text = data.note;//111111111111

            //setDataMillToRB(data.mill);
            setDataPayToRB(data.payType);
            setDataVatToRB(data.vatType);
            setDataCleanToRB(data.clean);

            AfterGetDataFromTable();

            //disable after read in
            disableBtAfterRead(1);

            //if user admin enable all
            if (Globals.isPermissionEditWeight())
            {
                disableBtAfterRead(999);
            }

            rbWeightOut.Checked = true;
        }

        private string tonTokg(string tonStr)
        {
            double tmp = Convert.ToDouble(tonStr);
            double deci = tmp * 1000;
            string str = deci.ToString("#,##0.00");
            return str;
        }

        private string numberFormat(string numStr, int format)
        {
            double deci = Convert.ToDouble(numStr);
            string str = "";
            if (format == 1)
                str = deci.ToString();
            else if (format == 2)
                str = deci.ToString("#,##0.00");
            return str;
        }

        private void setDataMillToRB(string dataMill) {
            if (dataMill.Equals("โรงโม่ 1"))
                rbMill1.Checked = true;
            else if (dataMill.Equals("โรงโม่ 2"))
                rbMill2.Checked = true;
            else if (dataMill.Equals("โรงโม่ 3"))
                rbMill3.Checked = true;
            else if (dataMill.Equals("ไม่มี"))
                rbMillNo.Checked = true;
        }
        private void setDataPayToRB(string dataPay)
        {
            if (dataPay.Equals("เงินสด"))
                rbCash.Checked = true;
            else if (dataPay.Equals("เงินเชื่อ"))
                rbCredit.Checked = true;
            else if (dataPay.Equals("เงินโอน"))
                rbTrans.Checked = true;
            else if (dataPay.Equals("Vat"))
                rbVat.Checked = true;
        }

        private void setDataVatToRB(string dataVat)
        {
            if (dataVat.Equals("ไม่รวมภาษี"))
                rbbVat.Checked = true;
            else if (dataVat.Equals("รวมภาษี"))
                rbbNonVat.Checked = true;
        }

        private void setDataCleanToRB(string dataClean)
        {
            if (dataClean.Equals("ล้างหิน"))
                rbCleanStone.Checked = true;
            else if (dataClean.Equals("สเปรย์น้ำ"))
                rbCleanWater.Checked = true;
            else if (dataClean.Equals("ไม่มี"))
                rbCleanNo.Checked = true;
        }

        private void setDefaultFromDB(string username, String firstname)
        {
            btMenu2.BackColor = Color.Thistle;
            btMenu3.BackColor = Color.Thistle;
            btMenu4.BackColor = Color.Thistle;
            btMenu5.BackColor = Color.Thistle;
            ucTruck.Show();
            ucReport.Hide();
            ucHelp.Hide();
            ucSetting.Hide();
            ucBackup.Hide();
            ucTruck.BringToFront();

            tbScaleId.Text = username;
            tbScaleName.Text = firstname;

            if (Globals.isPermissionSales()) {
                btMenu1.Enabled = false;
                btMenu3.Enabled = false;
            }

            if (!Globals.isPermissionAddSetting())
            {
                btMenu3.Enabled = false;
                btLoadCustomer.Enabled = false;
            }

        }

        private void MainForm_Load(object sender, EventArgs e)
        {


        }

        private void UserInitialization()
        {
            //Serial Port
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            /*
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));
            */

            ucHelp.SetSerialPortManager(_spManager);

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_spManager != null)
            {
                _spManager.Dispose();
            }
        }

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 1000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            // This application is connected to a GPS sending ASCCI characters, so data is converted to text
            string str = Encoding.ASCII.GetString(e.Data);
            tbData.AppendText(str);
            tbData.ScrollToCaret();

            try
            {
                //แสดงเลขน้ำหนักที่กำลังวิ่ง
                /* เครื่องพี่จ๋า */
                
                string newString = tbData.Text.Remove(tbData.Text.LastIndexOf("kg"));
                string remainingText = newString.Substring(newString.LastIndexOf("\r"));
                MatchCollection mc = Regex.Matches(remainingText, @"\d+");
                

                /* เครื่องพี่รุ่ง */
                //MatchCollection mc = Regex.Matches(str, @"\d+");

                if (mc.Count > 0)
                {
                    if (String.Compare(tbWeigtData.Text, mc[0].Value) != 0)
                    {
                        tbWeigtData.Text = mc[0].Value.TrimStart('0').PadLeft(1, '0');
                        //tbWeigtData.ForeColor = Color.LightCoral;
                    }
                    else
                    {
                        tbWeigtData.ForeColor = Color.LightGreen;
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        // Handles the "Start Listening"-buttom click event
        private void btnStart_Click(object sender, EventArgs e)
        {
            _spManager.StartListening();
        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click(object sender, EventArgs e)
        {
            _spManager.StopListening();
        }

        private void btRead_Click(object sender, EventArgs e)
        {
            try
            {
                _spManager.StopListening();

                /*
                int length = tbData.Text.Length;
                string substring = tbData.Text.Substring(length - 15, 7);
                tbWeightIn.Text = Regex.Match(substring, @"\d+").Value;
                tbData.Text = "";
                */

                tbWeightIn.Text = numberFormat(tbWeigtData.Text, 2);

                calculateWeight();
                _spManager.StartListening();

                dtWeightInTime.Text = DateTime.Now.ToShortTimeString();

                //disable after read in
                if (!Globals.isPermissionTop())
                    disableBtAfterRead(1);
            }
            catch (Exception) {
            }
        }

        /* 
         * mode 0 -> enable all
         * mode 1 -> disable after read in
         * mode 2 -> disable after read out
         */
        private void disableBtAfterRead(int mode) {
            if (mode.Equals(0))
            {
                tbCarLicense.Enabled = true;
                tbCarLicenseId.Enabled = true;
                tbCarCity.Enabled = true;

                //39 ไม่เปิดให้คีย์ นน. tbWeightIn.Enabled = true;
            }
            else if (mode.Equals(1))
            {
                disableReadWeightIn();
                dtWeightInDate.Enabled = false;
                dtWeightInTime.Enabled = false;

                if (!checkZeroStr(tbWeightIn.Text))
                    tbWeightIn.Enabled = false;
                if (!checkEmptyTB(tbCarLicense))
                {
                    tbCarLicenseId.Enabled = false;
                    tbCarLicense.Enabled = false;
                    tbCarCity.Enabled = false;
                }
            }
            else if (mode.Equals(2))
            {
                disableReadWeightOut();
                dtWeightOutDate.Enabled = false;
                dtWeightOutTime.Enabled = false;

                tbWeightOut.Enabled = false;
                tbWeightOut.Enabled = false;
                tbWeightTotal.Enabled = false;
                tbQ.Enabled = false;
            }
            else if (mode.Equals(4))//disable all
            {
                dtWeightInDate.Enabled = false;
                dtWeightInTime.Enabled = false;

                dtWeightOutDate.Enabled = false;
                dtWeightOutTime.Enabled = false;

                tbWeightIn.Enabled = false;
                tbWeightOut.Enabled = false;
                tbWeightTotal.Enabled = false;
                tbQ.Enabled = false;
            }

            else if (mode.Equals(999)) {
                tbWeightIn.Enabled = true;
                tbWeightOut.Enabled = true;
                tbWeightTotal.Enabled = true;
                tbQ.Enabled = true;

                tbCarLicense.Enabled = true;
                tbCarCity.Enabled = true;
            }
        }

        private Boolean checkEmptyTB(TextBox tb)
        {
            return string.IsNullOrEmpty(tb.Text) == true ? true : false;
        }



        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btMenu1_Click(object sender, EventArgs e)
        {
            btMenu1.BackColor = Color.White;
            btMenu2.BackColor = Color.Thistle;
            btMenu3.BackColor = Color.Thistle;
            btMenu4.BackColor = Color.Thistle;
            btMenu5.BackColor = Color.Thistle;
            /*
            ucTruck.Show();
            ucReport.Hide();
            ucHelp.Hide();
            ucSetting.Hide();
            ucBackup.Hide();
            ucTruck.BringToFront();
            
            TableFromDB mf = new TableFromDB(this);
            mf.ShowDialog();
            */
            clickWeightIn();

        }

        private void btMenu2_Click(object sender, EventArgs e)
        {
            btMenu2.BackColor = Color.White;
            btMenu1.BackColor = Color.Thistle;
            btMenu3.BackColor = Color.Thistle;
            btMenu4.BackColor = Color.Thistle;
            btMenu5.BackColor = Color.Thistle;

            ucReport.Show();
            ucTruck.Hide();
            ucHelp.Hide();
            ucSetting.Hide();
            ucBackup.Hide();
            ucReport.BringToFront();

        }
        private void btMenu3_Click(object sender, EventArgs e)
        {
            btMenu3.BackColor = Color.White;
            btMenu1.BackColor = Color.Thistle;
            btMenu2.BackColor = Color.Thistle;
            btMenu4.BackColor = Color.Thistle;
            btMenu5.BackColor = Color.Thistle;

            ucSetting.Show();
            ucReport.Hide();
            ucHelp.Hide();
            ucTruck.Hide();
            ucBackup.Hide();
            ucSetting.BringToFront();
        }
        private void btMenu4_Click(object sender, EventArgs e)
        {
            btMenu4.BackColor = Color.White;
            btMenu1.BackColor = Color.Thistle;
            btMenu2.BackColor = Color.Thistle;
            btMenu3.BackColor = Color.Thistle;
            btMenu5.BackColor = Color.Thistle;

            ucHelp.Show();
            ucTruck.Hide();
            ucReport.Hide();
            ucSetting.Hide();
            ucBackup.Hide();
            ucHelp.BringToFront();
        }

        private void btMenu5_Click(object sender, EventArgs e)
        {
            btMenu5.BackColor = Color.White;
            btMenu1.BackColor = Color.Thistle;
            btMenu2.BackColor = Color.Thistle;
            btMenu3.BackColor = Color.Thistle;
            btMenu4.BackColor = Color.Thistle;

            ucBackup.Show();
            ucHelp.Hide();
            ucTruck.Hide();
            ucReport.Hide();
            ucSetting.Hide();
            ucBackup.BringToFront();
        }



        private void pnHelp_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ucHelp_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void btReadOut_Click(object sender, EventArgs e)
        {
            try {
                _spManager.StopListening();

                /*
                int length = tbData.Text.Length;
                string substring = tbData.Text.Substring(length - 15, 7);
                tbWeightOut.Text = Regex.Match(substring, @"\d+").Value;
                */
                tbWeightOut.Text = numberFormat(tbWeigtData.Text, 2);

                calculateWeight();
                _spManager.StartListening();

                dtWeightOutTime.Text = DateTime.Now.ToShortTimeString();

                //disable after read out
                if (!Globals.isPermissionTop())
                    disableBtAfterRead(2);
            }
            catch (Exception) {
            }
        }

        private void calculateWeight() {
            string weightIn = tbWeightIn.Text;
            string weightOut = tbWeightOut.Text;
            double numWeightIn = 0;
            double numWeightOut = 0;

            if (weightIn != "" && weightIn != null && weightOut != "" && weightOut != null) {
                try {

                    numWeightIn = Convert.ToDouble(weightIn);
                    numWeightOut = Convert.ToDouble(weightOut);
                    double numWeight = 0;
                    if (numWeightIn > numWeightOut)
                        numWeight = numWeightIn - numWeightOut;
                    else if (numWeightIn < numWeightOut)
                        numWeight = numWeightOut - numWeightIn;
                    tbWeightTotal.Text = numWeight.ToString("#,##0.00");

                }
                catch (Exception) {
                }
            }

        }

        private Boolean checkDuplicateRunningNumber() {
            Boolean isDuplicate = false;
            string todayYear = DateTime.Now.ToString("yyyy");
            string startDate = todayYear + "-01-01";
            string endDate = todayYear + "-12-31";

            //sql get weight id
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT เลขที่เอกสาร FROM weight WHERE เลขที่เอกสาร = '" + tbDocNum.Text + "' AND วันที่ BETWEEN '" + startDate + "' AND '" + endDate + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                     isDuplicate = true;
                    //MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
            }
            dl.close();
            return isDuplicate;
        }

        private Boolean checkDuplicateCarLicense()
        {
            Boolean isDuplicate = false;
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            //sql get weight id
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT ทะเบียนรถ FROM weight WHERE ทะเบียนรถ = '" + tbCarLicense.Text + "' AND วันที่ = '" + today + "' AND น้ำหนักรวม = '0.00' AND NOT รหัสลูกค้า = '99' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    isDuplicate = true;
                    //MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
            }
            dl.close();
            return isDuplicate;
        }


        private void btSave_Click(object sender, EventArgs e)
        {
              autoSave();
        }

        private void autoSave()
        {

            Boolean isPasswordCorrect = true;
            if (tbCarLicenseId.Text == "" || tbCarLicense.Text == "") {
                MessageBox.Show("ทะเบียนรถเป็นค่าว่าง กรุณาเลือกหน้าต่างชั่งน้ำหนักเข้า หรือใส่เลขทะเบียนรถ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (tbId.Text == "")
            {
                isPasswordCorrect = checkCancelAction();

                //เช็คค่าว่าง
                if (tbDocNum.Text == "")
                    MessageBox.Show("เลขที่การชั่งเป็นค่าว่าง กรุณใส่เลขที่การชั่ง", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //เช็คเลขซ้ำกัน
                else if (checkDuplicateRunningNumber())
                    MessageBox.Show("เลขที่การชั่งนี้ใช้ไปแล้ว กรุณาเข้าหน้าต่างใหม่", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //เช็คเลขทะเบียนซ้ำกัน
                else if (checkDuplicateCarLicense())
                    MessageBox.Show("ทะเบียนรถนี้ยังไม่มีการชั่งออก กรุณาทำการชั่งใหม่", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else if (checkWeightInZero())
                    MessageBox.Show("น้ำหนักชั่งเข้าเป็น 0.00 ไม่สามารถบันทึกข้อมูลได้", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    saveAction();
            }
            else
            {
                isPasswordCorrect = checkCancelAction();
                if (isPasswordCorrect)
                    updateAction();
                else
                    MessageBox.Show("รหัสยกเลิกผิด ไม่สามารถบันทึกข้อมูลได้", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Boolean checkWeightInZero()
        {
            string str = tbWeightIn.Text;
            Double temp;
            Boolean isOk = Double.TryParse(str, out temp);
            Int32 value = isOk ? (Int32)temp : 0;

            return value == 0 ? true : false;
        }

        private void saveAction() {
            Boolean isSuccess = false;
            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "INSERT INTO weight (วันที่, เลขที่เอกสาร, ทะเบียนรถ, จังหวัด, คนขับ, ลูกค้า, น้ำหนักรถ, น้ำหนักรวม, น้ำหนักสินค้า , เลขที่ใบตัก, โรงโม่, ชนิดหิน, จ่ายเงิน, รหัสผู้ชั่ง, รหัสผู้ตัก, ราคาตัน, จำนวณเงิน, ค่าขนส่ง, วันที่ชั่งเข้า, เวลาชั่งเข้า, วันที่ชั่งออก, เวลาชั่งออก, รหัสลูกค้า, ชื่อผู้ชั่ง, ชื่อผู้ตัก, vat, รหัสผู้อนุมัติจ่าย, ชื่อผู้อนุมัติจ่าย, คิว, จำนวนเงินสุทธิ, ประเภทหิน, หน้างาน, ทีม, ล้าง, ขนส่ง, รหัสคนขับ, รหัสทะเบียนรถ, base_weight_station_name,bws, stone_type_id, mill_id, หมายเหตุ, site_id)" +
                                     "VALUES ('" + dtDate.Value.ToString("yyyy-MM-dd") + "','" + tbDocNum.Text + "','" + tbCarLicense.Text + "','" + tbCarCity.Text + "','" + tbDriverName.Text + "','" + tbCustomerName.Text + "','" + kgToTon(tbWeightIn) + "'" + ",'"
                                     + kgToTon(tbWeightOut) + "','" + kgToTon(tbWeightTotal) + "','" + tbRefNum.Text + "','" + tbMillName.Text + "','" + tbStoneTypeName.Text + "','" + getPayRadioValue() + "','" + tbScaleId.Text + "','"
                                     + tbScoopId.Text + "','" + numberFormat(tbPricePerTon.Text,1) + "','" + numberFormat(tbAmount.Text,1) + "','" + tbShipCost.Text + "','" + dtWeightInDate.Value.ToString("yyyy-MM-dd") + "','" + dtWeightInTime.Text + "','" + dtWeightOutDate.Value.ToString("yyyy-MM-dd") + "','" + dtWeightOutTime.Text + "','"
                                     + tbCustomerId.Text + "','" + tbScaleName.Text + "','" + tbScoopName.Text + "','" + numberFormat(tbVat.Text,1) + "','" + tbApproveId.Text + "','" + tbApproveName.Text + "','" + numberFormat(tbQ.Text,1) + "','" + numberFormat(tbAmountVat.Text,1) + "','"
                                     + cbbStoneColor.Text + "','" + tbSiteName.Text + "','" + tbCarTeam.Text + "','" + getCleanRadioValue() + "','" + cbbTransport.Text + "','" + tbDriverId.Text + "','" + tbCarLicenseId.Text + "', (SELECT base_weight_station_name FROM base_weight_station WHERE base_weight_station_id = 1 ) , (SELECT code FROM base_weight_station WHERE base_weight_station_id = 1 ) ,'"
                                     + tbStoneTypeId.Text + "','" + tbMillId.Text + "','" + tbNote.Text + "','" + tbSiteId.Text + "' )";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                isSuccess = runningDocNumberAfterSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            dl.close();

            //set WeightId
            if (isSuccess)
                setWeightId();

            //ปิดช่องหลัง save
            disableAfterSave();
        }

        private void saveWeightHistory()
        {

            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "INSERT INTO weight_log (วันที่, เลขที่เอกสาร, ทะเบียนรถ, จังหวัด, คนขับ, ลูกค้า, น้ำหนักรถ, น้ำหนักรวม, น้ำหนักสินค้า , เลขที่ใบตัก, โรงโม่, ชนิดหิน, จ่ายเงิน, รหัสผู้ชั่ง, รหัสผู้ตัก, ราคาตัน, จำนวณเงิน, ค่าขนส่ง, วันที่ชั่งเข้า, เวลาชั่งเข้า, วันที่ชั่งออก, เวลาชั่งออก, รหัสลูกค้า, ชื่อผู้ชั่ง, ชื่อผู้ตัก, vat, รหัสผู้อนุมัติจ่าย, ชื่อผู้อนุมัติจ่าย, คิว, จำนวนเงินสุทธิ, ประเภทหิน, หน้างาน, ทีม, ล้าง, ขนส่ง, รหัสคนขับ, รหัสทะเบียนรถ, base_weight_station_name, stone_type_id, mill_id, หมายเหตุ, site_id)" +
                                     "VALUES ('" + dtDate.Value.ToString("yyyy-MM-dd") + "','" + tbDocNum.Text + "','" + tbCarLicense.Text + "','" + tbCarCity.Text + "','" + tbDriverName.Text + "','" + tbCustomerName.Text + "','" + kgToTon(tbWeightIn) + "'" + ",'"
                                     + kgToTon(tbWeightOut) + "','" + kgToTon(tbWeightTotal) + "','" + tbRefNum.Text + "','" + tbMillName.Text + "','" + tbStoneTypeName.Text + "','" + getPayRadioValue() + "','" + tbScaleId.Text + "','"
                                     + tbScoopId.Text + "','" + numberFormat(tbPricePerTon.Text, 1) + "','" + numberFormat(tbAmount.Text, 1) + "','" + tbShipCost.Text + "','" + dtWeightInDate.Value.ToString("yyyy-MM-dd") + "','" + dtWeightInTime.Text + "','" + dtWeightOutDate.Value.ToString("yyyy-MM-dd") + "','" + dtWeightOutTime.Text + "','"
                                     + tbCustomerId.Text + "','" + tbScaleName.Text + "','" + tbScoopName.Text + "','" + numberFormat(tbVat.Text, 1) + "','" + tbApproveId.Text + "','" + tbApproveName.Text + "','" + numberFormat(tbQ.Text, 1) + "','" + numberFormat(tbAmountVat.Text, 1) + "','"
                                     + cbbStoneColor.Text + "','" + tbSiteName.Text + "','" + tbCarTeam.Text + "','" + getCleanRadioValue() + "','" + cbbTransport.Text + "','" + tbDriverId.Text + "','" + tbCarLicenseId.Text + "', (SELECT base_weight_station_name FROM base_weight_station WHERE base_weight_station_id = 1 ) ,'"
                                     + tbStoneTypeId.Text + "','" + tbMillId.Text + "','" + tbNote.Text + "','" + tbSiteId.Text + "' )";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            dl.close();

        }

        private void setWeightId() {
            //sql get weight id
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT weight_id FROM public.weight WHERE เลขที่เอกสาร = '" + tbDocNum.Text + "' AND วันที่ = '" + dtDate.Value.ToString("yyyy-MM-dd") + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string rdStr = reader["weight_id"].ToString();
                    tbId.Text = rdStr;
                    //MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void updateAction()
        {
            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "UPDATE weight SET ทะเบียนรถ = '" + tbCarLicense.Text + "' , จังหวัด = '" + tbCarCity.Text + "' , คนขับ = '" + tbDriverName.Text + "', ลูกค้า = '" + tbCustomerName.Text + "' , น้ำหนักรถ = '" + kgToTon(tbWeightIn) + "' , น้ำหนักรวม = '" + kgToTon(tbWeightOut) + "'" +
                                    " , น้ำหนักสินค้า = '" + kgToTon(tbWeightTotal) + "' , เลขที่ใบตัก = '" + tbRefNum.Text + "' , โรงโม่ = '" + tbMillName.Text + "' , ชนิดหิน = '" + tbStoneTypeName.Text + "' , จ่ายเงิน = '" + getPayRadioValue() + "' , รหัสผู้ชั่ง = '" + tbScaleId.Text + "'" +
                                    " , รหัสผู้ตัก = '" + tbScoopId.Text + "' , ราคาตัน = '" + numberFormat(tbPricePerTon.Text, 1) + "' , จำนวณเงิน = '" + numberFormat(tbAmount.Text, 1) + "' , ค่าขนส่ง = '" + tbShipCost.Text + "' , วันที่ชั่งเข้า = '" + dtWeightInDate.Value.ToString("yyyy-MM-dd") + "' , เวลาชั่งเข้า = '" + dtWeightInTime.Text + "'" +
                                    " , วันที่ชั่งออก = '" + dtWeightOutDate.Value.ToString("yyyy-MM-dd") + "' , เวลาชั่งออก = '" + dtWeightOutTime.Text + "'  , รหัสลูกค้า = '" + tbCustomerId.Text + "'  , ชื่อผู้ชั่ง = '" + tbScaleName.Text + "' , ชื่อผู้ตัก = '" + tbScoopName.Text + "' , vat = '" + numberFormat(tbVat.Text, 1) +
                                    "' , รหัสผู้อนุมัติจ่าย = '" + tbApproveId.Text + "' , ชื่อผู้อนุมัติจ่าย = '" + tbApproveName.Text + "' , คิว = '" + numberFormat(tbQ.Text, 1) + "' , ชนิดvat = '" + getVatRadioValue() + "' , จำนวนเงินสุทธิ = '" + numberFormat(tbAmountVat.Text, 1) + "' , ประเภทหิน = '" + cbbStoneColor.Text +
                                    "' , site_id = '" + tbSiteId.Text + "' , stone_type_id = '" + tbStoneTypeId.Text + "' , mill_id = '" + tbMillId.Text + "' , หมายเหตุ = '" + tbNote.Text +
                                    "' , หน้างาน = '" + tbSiteName.Text + "' , ทีม = '" + tbCarTeam.Text + "' , ล้าง = '" + getCleanRadioValue() + "' , ขนส่ง = '" + cbbTransport.Text + "' , รหัสคนขับ = '" + tbDriverId.Text + "' , รหัสทะเบียนรถ = '" + tbCarLicenseId.Text + "' WHERE วันที่ = '" + dtDate.Value.ToString("yyyy-MM-dd") + "' AND weight_id = " + tbId.Text + " ; ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                //MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                while (reader.Read())
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            dl.close();

            //ปิดช่องหลัง save
            disableAfterSave();
        }

        private void disableAfterSave()
        {
            if (!Globals.isPermissionEditWeight())
            {
                if (!checkZeroStr(tbWeightIn.Text))
                    disableBtAfterRead(1);
                if (!checkZeroStr(tbWeightOut.Text))
                    disableBtAfterRead(2);
            }


            //รหัสยกเลิกให้ปิดช่องให้หมด
            disableCancelId();

            //19-09-2023 มาเก็บ weight history ตรงนี้นะ
            saveWeightHistory();
        }

        private void disableCancelId()
        {
            if (tbCustomerId.Text == "99RM" || tbCustomerId.Text == "99")
            {
                disableBtAfterRead(4);
                if (checkEmptyTB(tbNote))
                {
                    MessageBox.Show("กรุณาใส่เหตุผลในการยกเลิก", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbNote.Select();
                }
                updateStatusCancel(true);
            }
            else {
                updateStatusCancel(false);
            }
        }

        private void updateStatusCancel(Boolean status)
        {
            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "UPDATE weight SET is_cancel =  "+ status  + "  WHERE วันที่ = '" + dtDate.Value.ToString("yyyy-MM-dd") + "' AND weight_id = " + tbId.Text + " ; ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                //MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                while (reader.Read())
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            dl.close();
        }

        private Boolean checkZeroStr(string str)
        {
            Double temp;
            Boolean isOk = Double.TryParse(str, out temp);
            Int32 value = isOk ? (Int32)temp : 0;

            return value == 0 ? true : false;
        }

        //get combobox id use to save or update
        private string getComboboxId(ComboBox cbb)
        {
            string tmp = "";

            if (cbb.SelectedIndex > -1)
            {
                ComboboxValue tmpComboboxValue = (ComboboxValue)cbb.SelectedItem;
                tmp = tmpComboboxValue.Id;
            }
            return tmp;
        }

        private string getMillId() {
            string mill_id = "";
            if (rbMillNo.Checked)
                mill_id = "00";
            else if (rbMill1.Checked)
                mill_id = "01";
            else if (rbMill2.Checked)
                mill_id = "02";
            else if (rbMill3.Checked)
                mill_id = "03";
            return mill_id;
        }

        private string getComboboxSiteUpdate()
        {

            string selectedName = cbbSite.Text;
            string selectedId = "";

            foreach (ComboboxValue item in cbbSite.Items)
            {

                if (item.Name == selectedName)
                {
                    selectedId = item.Id;
                    break;
                }
            }
            return selectedId;
        }

        private string getComboboxStoneTypeUpdate()
        {

            string selectedName = cbbStoneType.Text;
            string selectedId = "";

            foreach (ComboboxValue item in cbbStoneType.Items)
            {

                if (item.Name == selectedName)
                {
                    selectedId = item.Id;
                    break;
                }
            }
            return selectedId;
        }

        private string getComboboxMillUpdate()
        {

            string selectedName = cbbMill.Text;
            string selectedId = "";

            foreach (ComboboxValue item in cbbMill.Items)
            {

                if (item.Name == selectedName)
                {
                    selectedId = item.Id;
                    break;
                }
            }
            return selectedId;
        }

        private Boolean runningDocNumberAfterSave()
        {
            Boolean isSuccess = false;
            string todayYear = DateTime.Now.ToString("yyyy");
            //sql find
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public.seq_doc_num where run_year = '"+ todayYear + "'";
            try
            {

                //sql update
                pgCommand.CommandText = "UPDATE public.seq_doc_num SET run_number = '" + tbDocNum.Text + "' where run_year = '" + todayYear + "'";
                OdbcDataReader reader = pgCommand.ExecuteReader();
                isSuccess = true;

            }
            catch (Exception)
            {
            }
            return isSuccess;
        }

        private string getMillRadioValue() {
            string value = "";
            if (rbMill1.Checked)
                value = rbMill1.Text;
            else if (rbMill2.Checked)
                value = rbMill2.Text;
            else if (rbMill3.Checked)
                value = rbMill3.Text;
            else if (rbMillNo.Checked)
                value = rbMillNo.Text;
            return value;
        }

        private string getCleanRadioValue()
        {
            string value = "";
            if (rbCleanStone.Checked)
                value = rbCleanStone.Text;
            else if (rbCleanWater.Checked)
                value = rbCleanWater.Text;
            else if (rbCleanNo.Checked)
                value = rbCleanNo.Text;
            return value;
        }
        private string getPayRadioValue()
        {
            string value = "";
            if (rbCash.Checked)
                value = rbCash.Text;
            else if (rbCredit.Checked)
                value = rbCredit.Text;
            if (rbTrans.Checked)
                value = rbTrans.Text;
            return value;
        }

        private string getVatRadioValue()
        {
            string value = null;
            if (rbbNonVat.Checked)
                value = rbbNonVat.Text;
            else if (rbbVat.Checked)
                value = rbbVat.Text;
            return value;
        }

        private string getVatRadioValuePrint()
        {
            string value = null;
            if (rbbNonVat.Checked)
            {
                value = "ใบส่งของ";
                Company.CompanyName = " ";
                Company.Address = " ";
                Company.Email = " ";
                Company.Telephone = " ";
                Company.TTelephone = " ";
                Company.TEmail = " ";
            }
            else if (rbbVat.Checked)
            {
                value = "ใบส่งสินค้า";
                getDefaultCompany();
            }
            else
            {
                value = "";
            }
            return value;
        }



        private void getDefaultCompany()
        {
            //sql find company
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public.base_company where base_company_id = 1 ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    Company.CompanyName = reader["company_name"].ToString();
                    Company.Address = reader["address"].ToString();
                    Company.Telephone = reader["telephone"].ToString();
                    Company.Email = reader["email"].ToString();
                }
            }
            catch (Exception)
            {
            }
            dl.close();

        }


        /* autoComplete Setting */
        private void autoCompleteSetting(TextBox tb, string field, string tableName)
        {
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public." + tableName;
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string rdStr = reader[field].ToString();
                    coll.Add(rdStr);
                }
            }
            catch (Exception) {
            }
            tb.AutoCompleteCustomSource = coll;
            dl.close();
        }

        /* autoComplete Setting By Company*/
        private void autoCompleteSettingByCompany(TextBox tb, string field, string tableName)
        {
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public." + tableName + " where company = '"+ Company.Code + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string rdStr = reader[field].ToString();
                    coll.Add(rdStr);
                }
            }
            catch (Exception)
            {
            }
            tb.AutoCompleteCustomSource = coll;
            dl.close();
        }

        /* autoComplete Setting By Weight Type*/
        private void autoCompleteSettingByWeightType(TextBox tb, string field, string tableName)
        {
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public." + tableName + " where weight_type = 2 or  weight_type = 3 ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string rdStr = reader[field].ToString();
                    coll.Add(rdStr);
                }
            }
            catch (Exception)
            {
            }
            tb.AutoCompleteCustomSource = coll;
            dl.close();
        }

        /* autoComplete Setting */
        private void autoCompleteSettingInactive(TextBox tb, string field, string tableName)
        {
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public." + tableName + " where inactive = false ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string rdStr = reader[field].ToString();
                    coll.Add(rdStr);

                }
            }
            catch (Exception)
            {
            }
            tb.AutoCompleteCustomSource = coll;
            dl.close();
        }

        private void tbCustomerName_TextChanged(object sender, EventArgs e)
        {
            //customerNameTextChanged();
        }

        private void customerNameTextChanged()
        {
            if (tbCustomerName != null && tbCustomerName.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_customer where ชื่อลูกค้า = '" + tbCustomerName.Text + "' and weight_type = 2 or weight_type = 3";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["รหัสลูกค้า"].ToString();
                        tbCustomerId.Text = rdStr;
                    }

                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("ไม่มีชื่อลูกค้า " + tbCustomerName.Text, "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tbCustomerId.Text = "";
                        tbCustomerName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
                Weight.CustomerAddress = getPrintFromDB("base_customer", "ที่อยู่", "รหัสลูกค้า", tbCustomerId.Text);
            }
            else
            {
                tbCustomerId.Text = "";
                Weight.CustomerAddress = " ";
            }
        }

        private void tbCustomerId_TextChanged(object sender, EventArgs e)
        {
            //customerIdTextChanged();
        }

        private void customerIdTextChanged()
        {
            if (tbCustomerId != null && tbCustomerId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_customer where รหัสลูกค้า = '" + tbCustomerId.Text + "' and weight_type = 2 or  weight_type = 3";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อลูกค้า"].ToString();
                        tbCustomerName.Text = rdStr;
                    }

                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("ไม่มีรหัสลูกค้า " + tbCustomerId.Text, "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tbCustomerId.Text = "";
                        tbCustomerName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
                Weight.CustomerAddress = getPrintFromDB("base_customer", "ที่อยู่", "รหัสลูกค้า", tbCustomerId.Text);
            }
            else
            {
                tbCustomerName.Text = "";
                Weight.CustomerAddress = " ";
            }
        }



        private void tbScoopId_TextChanged(object sender, EventArgs e)
        {

        }
        private void tbScoopName_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbScaleId_TextChanged(object sender, EventArgs e)
        {
            if (tbScaleId != null && tbScaleId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.users where username = '" + tbScaleId.Text + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["firstname"].ToString();
                        tbScaleName.Text = rdStr;
                    }
                }
                catch (Exception) {
                }
                dl.close();
            }
            else {
                tbScaleName.Text = "";
            }
        }

        private void tbScaleName_TextChanged(object sender, EventArgs e)
        {
            if (tbScaleName != null && tbScaleName.Text != "") {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.users where firstname = '" + tbScaleName.Text + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["username"].ToString();
                        tbScaleId.Text = rdStr;
                    }
                }
                catch (Exception) {
                }
                dl.close();
            }
            else {
                tbScaleId.Text = "";
            }

        }

        private void tbPricePerTon_TextChanged(object sender, EventArgs e)
        {
            calculateAmount();
            calculateVat();
        }
        private void tbWeightTotal_TextChanged(object sender, EventArgs e)
        {
            calculateAmount();
            calculateVat();
            calculatenumQ();
        }

        private void calculateAmount() {
            try {
                double total = 0;
                total = Convert.ToDouble(tbWeightTotal.Text);
                double price = 0;
                price = Convert.ToDouble(tbPricePerTon.Text);
                double amount = 0;
                amount = (total / 1000) * price;
                tbAmountVat.Text = amount.ToString("#,##0.00");

                //set Temp
                tbAmount.Text = tbAmountVat.Text;
            }
            catch (Exception ex) {
                //MessageBox.Show(ex.ToString());
            }
        }

        private void calculatenumQ() {
            try
            {
                if (!checkZeroStr(tbWeightIn.Text) && !checkZeroStr(tbWeightOut.Text) && tbStoneTypeName != null && tbStoneTypeName.Text != "")
                {
                    double numCalQ = Convert.ToDouble(strCalQ);
                    double numWeightTotal = Convert.ToDouble(tbWeightTotal.Text);
                    double numQ = numWeightTotal / (numCalQ * 1000);
                    tbQ.Text = numQ.ToString("#,##0.00");
                }
                else
                {
                    tbQ.Text = "0.00";
                }

            }
            catch (Exception e)
            {

            }
        }

        private void btPrintIn_Click(object sender, EventArgs e)
        {
            //เช็คค่าว่าง
            showErrorWeightInEmty();

            //ปริ้น
            preparePrint(1);

            if (checkDuplicateRunningNumber() && tbId.Text == ""){
                //ไม่ต้องทำไร
            }
            else{
                FPrint f = new FPrint();
                f.ShowDialog();
            }
            //save อัตโนมัติ
            autoSave();
        }

        private void preparePrint(int mode) {
            Company.TTelephone = "โทร";
            Company.TEmail = "E-mail";
            Weight.Date = dtDate.Text;
            Weight.DocNum = tbDocNum.Text;
            Weight.Mill = strNotEmty(tbMillName.Text);
            Weight.DriverName = strNotEmty(tbDriverName.Text);
            Weight.CustomerName = strNotEmty(tbCustomerName.Text);
            Weight.CustomerAddress = getPrintFromDB("base_customer", "ที่อยู่", "รหัสลูกค้า", tbCustomerId.Text);
            Weight.StoneType = strNotEmty(tbStoneTypeName.Text);
            Weight.CarLicense = strNotEmty(tbCarLicense.Text);
            Weight.CarCity = strNotEmty(tbCarCity.Text);
            Weight.DateIn = strNotEmty(dtWeightInDate.Text);
            Weight.TimeIn = strNotEmty(dtWeightInTime.Text);
            Weight.DateOut = strNotEmty(dtWeightOutDate.Text);
            Weight.TimeOut = strNotEmty(dtWeightOutTime.Text);
            Weight.WeightIn = kgToTon(tbWeightIn);
            Weight.WeightOut = kgToTon(tbWeightOut);
            Weight.WeightTotal = kgToTon(tbWeightTotal);
            Weight.Price = tbPricePerTon.Text;
            Weight.Amount = tbAmount.Text;
            Weight.Vat = tbVat.Text;
            Weight.AmountVat = tbAmountVat.Text;
            Weight.Q = tbQ.Text;
            Weight.Team = strNotEmty(tbCarTeam.Text);
            Weight.StoneColor = strNotEmty(cbbStoneColor.Text);
            Weight.Site = strNotEmty(tbSiteName.Text);
            Weight.ApproveName = strNotEmty(tbApproveName.Text);
            Weight.Pay = strNotEmty(getPayRadioValue());
            Weight.VatType = strNotEmty(getVatRadioValuePrint());
            Weight.Clean = strNotEmty(getCleanRadioValue());
            Weight.Transport = strNotEmty(cbbTransport.Text);
            Weight.ScoopName = strNotEmty(tbScoopName.Text);
            Weight.Note = strNotEmty(tbNote.Text);

            HandleSuccessfulPrint();

            if (mode.Equals(3))
            {
                //ปริ้นทั้ง IN และ OUT
                Company.TDocName = "เลขที่การชั่ง";
            }
            else if (mode.Equals(2)) {
                //ปริ้น OUT
                Weight.Pay = " ";
                Weight.DocNum = " ";
                Weight.DateIn = " ";
                Weight.TimeIn = " ";
                Weight.WeightIn = " ";
                Weight.CustomerName = " ";
                Weight.CustomerAddress = " ";
                Weight.Site = " ";
                Weight.StoneType = " ";
                Weight.CarLicense = " ";
                Weight.CarCity = " ";
                Weight.DriverName = " ";
                Weight.Team = " ";
                Company.TDocName = " ";
            }
            else if (mode.Equals(1)) {
                //ปริ้น IN
                Weight.Mill = " ";
                Weight.StoneColor = " ";
                Weight.Clean = " ";                
                Weight.ApproveName = " ";
                Weight.DateOut = " ";
                Weight.TimeOut = " ";
                Weight.WeightOut = " ";
                Weight.WeightTotal = " ";
                Weight.Q = " ";
                Weight.Pay = " ";
                Weight.Price = " ";
                Weight.Amount = " ";
                Weight.Vat = " ";
                Weight.AmountVat = " ";
                Weight.Transport = " ";
                Company.TDocName = "เลขที่การชั่ง";
            }

        }

        private int findLastCopyByWeightId()
        {
            int copy_num = 0;

            if (tbId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "select copy_num from weight_copy where weight_id = '" + tbId.Text + "' ORDER BY weight_copy_id DESC LIMIT 1";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    if (reader.Read())
                    {
                        copy_num = Convert.ToInt32(reader["copy_num"].ToString());
                    }
                    else
                    {
                        copy_num = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                dl.close();
            }

            return copy_num;
        }


        private void HandleSuccessfulPrint()
        {
            int copy_num = findLastCopyByWeightId();
            copy_num++;

            Weight.DatePrint = DateTime.Now.ToString("yyyy-MM-dd");
            Weight.DatePrintAndCopyNum = DateTime.Now.ToString("dd/MM") + "#" + copy_num;
            Weight.TimePrint = DateTime.Now.ToString("HH:mm:ss");

            //save weight copy
            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "INSERT INTO weight_copy (copy_num, date_print, time_print, user_print, weight_id )" +
                                     "VALUES ('" + copy_num + "','" + Weight.DatePrint + "','" + Weight.TimePrint + "','" + Globals.UserId + "','" + tbId.Text + "' )";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            dl.close();
        }

        private string strNotEmty(string str) {
            return str == "" ? " " : str;
        }

        private string zeroNotEmty(string str)
        {
            return str == "0.00" || str == "0.000" ? " " : str;
        }

        private string getPrintFromDB(string database, string field, string fieldCondition, string condition) {
            //sql
            string rdStr = " ";
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT "+ field + " FROM public."+ database + " where "+fieldCondition+" = '" + condition + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    rdStr = reader[field].ToString();
                }
            }
            catch (Exception)
            {
            }
            dl.close();


            if (rdStr == null || rdStr == ""){
                rdStr = " ";
            }

            return rdStr;

        }

        private string kgToTon(TextBox tb)
        {
            double tmp = Convert.ToDouble(tb.Text);
            double deci = tmp/1000;
            string str = string.Format("{0:0.000}", deci);
            return str;
        }

        private void btLoadCustomer_Click(object sender, EventArgs e)
        {
            TableCustomer tc = new TableCustomer(this);
            tc.ShowDialog();
        }

        private void cbbStoneType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT ค่าคำนวณคิว FROM public.base_stone_type where ชื่อหิน = '" + cbbStoneType.Text + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string rdStr = reader["ค่าคำนวณคิว"].ToString();
                    strCalQ = rdStr;
                }
            }
            catch (Exception)
            {
            }
            dl.close();

            //Weight.StoneColor = getPrintFromDB("base_stone_type", "ประเภทหิน", "ชื่อหิน", cbbStoneType.Text);
            //คำนวณค่าคิว
            calculatenumQ();
        }
        private void textboxFormatDecimal(object sender, KeyPressEventArgs e, TextBox textBox)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

            if (!char.IsControl(e.KeyChar))
            {

                textBox = (TextBox)sender;

                if (textBox.Text.IndexOf('.') > -1 &&
                         textBox.Text.Substring(textBox.Text.IndexOf('.')).Length >= 3)
                {
                    e.Handled = true;
                }

            }

        }

        private void tbPricePerTon_Leave(object sender, EventArgs e)
        {
            convertFormatToDecimal(tbPricePerTon);
        }

        private void rbbNonVat_CheckedChanged(object sender, EventArgs e)
        {
            if (rbbNonVat.Checked)
                tbAmountVat.Text = tbAmount.Text;
        }

        private void rbbVat_CheckedChanged(object sender, EventArgs e)
        {
            if (rbbVat.Checked)
            {
                try
                {
                    double tempAmount = Convert.ToDouble(tbAmount.Text);
                    double vat = (tempAmount * 7.0) / 100;
                    tbVat.Text = vat.ToString("#,##0.00");
                    double total = tempAmount + vat;
                    tbAmountVat.Text = total.ToString("#,##0.00");
                }
                catch (Exception ec)
                {

                }
            }
            else {
                tbVat.Text = "0.00";
            }
        }

        private void calculateVat() {
            if (rbbVat.Checked)
            {
                try
                {
                    double tempAmount = Convert.ToDouble(tbAmount.Text);
                    double vat = (tempAmount * 7.0) / 100;
                    tbVat.Text = vat.ToString("#,##0.00");
                    double total = tempAmount + vat;
                    tbAmountVat.Text = total.ToString("#,##0.00");
                }
                catch (Exception ec)
                {
                }
            }else if (rbbNonVat.Checked)
            {
                tbAmountVat.Text = tbAmount.Text;
                tbVat.Text = "0.00";
            }
        }

        private void tbApproveId_TextChanged(object sender, EventArgs e)
        {
            if (tbApproveId != null && tbApproveId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_approve where รหัสผู้อนุมัติจ่าย = '" + tbApproveId.Text + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อผู้อนุมัติจ่าย"].ToString();
                        tbApproveName.Text = rdStr;
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbApproveName.Text = "";
            }

        }

        private void tbApproveName_TextChanged(object sender, EventArgs e)
        {
            if (tbApproveName != null && tbApproveName.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_approve where ชื่อผู้อนุมัติจ่าย = '" + tbApproveName.Text + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["รหัสผู้อนุมัติจ่าย"].ToString();
                        tbApproveId.Text = rdStr;
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbApproveId.Text = "";
            }
        }

        private void label26_Click(object sender, EventArgs e)
        {

        }
        private void convertFormatToDecimal(TextBox tb) {
            try
            {
                double d = Convert.ToDouble(tb.Text);
                tb.Text = d.ToString("#,##0.00");
            }
            catch (Exception ex) {
                MessageBox.Show("ชนิดของข้อมูลผิด กรุณากรอกข้อมูลใหม่", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb.Text = "0.00";
            }
        }

        private void tbWeightTotal_Leave(object sender, EventArgs e)
        {
            convertFormatToDecimal(tbWeightTotal);
        }

        private void tbAmount_Leave(object sender, EventArgs e)
        {
            convertFormatToDecimal(tbAmount);
        }
        
        private void tbVat_Leave(object sender, EventArgs e)
        {
            convertFormatToDecimal(tbVat);
        }

        private void tbAmountVat_Leave(object sender, EventArgs e)
        {
            convertFormatToDecimal(tbAmountVat);
        }

        private void tbQ_Leave(object sender, EventArgs e)
        {
            convertFormatToDecimal(tbQ);
        }

        private void tbWeightOut_TextChanged(object sender, EventArgs e)
        {
            calculateWeight();
        }

        private void tbWeightIn_TextChanged(object sender, EventArgs e)
        {
            calculateWeight();
        }

        private void tbCarLicense_TextChanged(object sender, EventArgs e)
        {
            /*ค้นหาทีม ปิดไว้ก่อน
            if (tbCarLicense != null && tbCarLicense.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                //pgCommand.CommandText = "SELECT รหัสทีม FROM public.base_car where ชื่อรถร่วม = '" + tbCarLicense.Text + "' ";
                pgCommand.CommandText = "SELECT base_car_team.ชื่อทีม FROM base_car INNER JOIN base_car_team ON base_car.รหัสทีม = base_car_team.รหัสทีม WHERE base_car.ชื่อรถร่วม = '" + tbCarLicense.Text + "' ";
                try
                {
                    collCarTeam.Clear();
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อทีม"].ToString();
                        tbCarTeam.Text = rdStr;
                        collCarTeam.Add(rdStr);
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbCarTeam.Text = "";
            }
            */
        }


        private void tbCarTeam_Click(object sender, EventArgs e)
        {
            tbCarTeam.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tbCarTeam.AutoCompleteSource = AutoCompleteSource.CustomSource;
            tbCarTeam.AutoCompleteCustomSource = collCarTeam;  
        }

        private void timerWeight_Tick(object sender, EventArgs e)
        {
            tbWeigtData.Text = tbWeigtData.Text;
        }

        private Boolean checkCancelAction()
        {
            if (tbCustomerId.Text == "99RM" || tbCustomerId.Text == "99")
            {
                /* ใส่รหัสยกเลิก
                using (var form = new FCancelPassword())
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string password = form.ReturnPassword;
                        if (password == "pdg]bd=yj'")
                        {
                            tbWeightIn.Text = "0.00";
                            tbWeightOut.Text = "0.00";
                            tbWeightTotal.Text = "0.00";
                            tbPricePerTon.Text = "0.00";
                            tbAmount.Text = "0.00";
                            tbAmountVat.Text = "0.00";
                            tbVat.Text = "0.00";
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                }
                */

                tbWeightIn.Text = "0.00";
                tbWeightOut.Text = "0.00";
                tbWeightTotal.Text = "0.00";
                tbPricePerTon.Text = "0.00";
                tbAmount.Text = "0.00";
                tbAmountVat.Text = "0.00";
                tbVat.Text = "0.00";
                return true;
            }
            return true;
        }

        private void checkResetWeight() {
            if (tbCustomerId.Text == "99RM" || tbCustomerId.Text == "99") {
                tbWeightIn.Text = "0.00";
                tbWeightOut.Text = "0.00";
                tbWeightTotal.Text = "0.00";
                tbPricePerTon.Text = "0.00";
                tbAmount.Text = "0.00";
                tbAmountVat.Text = "0.00";
                tbVat.Text = "0.00";
            }
        }

        private void tbCustomerId_Leave(object sender, EventArgs e)
        {
            checkResetWeight();
            customerIdTextChanged();
        }

        private void tbCustomerName_Leave(object sender, EventArgs e)
        {
            checkResetWeight();
            customerNameTextChanged();
        }

        private void rbMill1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void rbMill1_Click(object sender, EventArgs e)
        {
            
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked)
            {
                radio.Checked = false;
            }


        }

        private void rbCash_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedCash = radio.Checked;
        }

        private void rbCash_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedCash)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedCash = false;
            }
        }

        private void rbTrans_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedTrans = radio.Checked;
        }

        private void rbTrans_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedTrans)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedTrans = false;
            }
        }

        private void rbCredit_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedCredit = radio.Checked;
        }

        private void rbCredit_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedCredit)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedCredit = false;
            }
        }

        private void rbMill1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedMill1 = radio.Checked;
        }

        private void rbMill1_Click_1(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedMill1)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedMill1 = false;
            }
        }

        private void rbMill2_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedMill2 = radio.Checked;
        }

        private void rbMill2_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedMill2)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedMill2 = false;
            }
        }

        private void rbMill3_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedMill3 = radio.Checked;
        }

        private void rbMill3_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedMill3)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedMill3 = false;
            }
        }

        private void rbCleanStone_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedCleanStone = radio.Checked;
        }

        private void rbCleanStone_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedCleanStone)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedCleanStone = false;
            }
        }

        private void rbCleanWater_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedCleanWater = radio.Checked;
        }

        private void rbCleanWater_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedCleanWater)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedCleanWater = false;
            }
        }

        private void btPrintOut_Click(object sender, EventArgs e)
        {
            //เช็คค่าว่าง
            showErrorWeightOutEmty();

            //ปริ้น
            preparePrint(2);
            if (checkDuplicateRunningNumber() && tbId.Text == ""){
                //ไม่ต้องทำไร
            }
            else{
                FPrint f = new FPrint();
                f.ShowDialog();
            }

            //save อัตโนมัติ
            autoSave();
        }

        private void btPrintAll_Click(object sender, EventArgs e)
        {
            //เช็คค่าว่าง
            //showErrorWeightInEmty();
            //showErrorWeightOutEmty();

            //ปริ้น
            preparePrint(3);
            if (checkDuplicateRunningNumber() && tbId.Text == ""){
                //ไม่ต้องทำไร
            }
            else {
                FPrint f = new FPrint();
                f.ShowDialog();
            }

            //save อัตโนมัติ
            autoSave();
        }

        private void rbMillNo_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedMillNo = radio.Checked;
        }

        private void rbMillNo_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedMillNo)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedMillNo = false;
            }
        }

        private void rbCleanNo_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            isCheckedCleanNo = radio.Checked;
        }

        private void rbCleanNo_Click(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton)sender;
            if (radio.Checked && !isCheckedCleanNo)
                radio.Checked = false;
            else
            {
                radio.Checked = true;
                isCheckedCleanNo = false;
            }
        }

        private void showErrorEmtyTextBox(TextBox tb)
        {
            if (string.IsNullOrEmpty(tb.Text) || tb.Text == "0.00")
                MessageBox.Show("' "+ tb.AccessibleName + "' เป็นค่าว่าง กรุณาใส่ข้อมูลให้ครบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void showErrorEmtyComboBox(ComboBox cbb)
        {
            if (string.IsNullOrEmpty(cbb.Text))
                MessageBox.Show("' " + cbb.AccessibleName + "' เป็นค่าว่าง กรุณาใส่ข้อมูลให้ครบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void showErrorEmtyRadioButton(GroupBox gb)
        {
            var rd = gb.Controls.OfType<RadioButton>()
                    .FirstOrDefault(n => n.Checked);
            if(rd == null)
                MessageBox.Show("' " + gb.AccessibleName + "' เป็นค่าว่าง กรุณาใส่ข้อมูลให้ครบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void showErrorWeightInEmty() {
            showErrorEmtyRadioButton(groupBox2);
            showErrorEmtyTextBox(tbStoneTypeName);
            showErrorEmtyComboBox(cbbStoneColor);
            showErrorEmtyTextBox(tbCarLicense);
            showErrorEmtyTextBox(tbCarCity);
            showErrorEmtyTextBox(tbWeightIn);
        }

        private void showErrorWeightOutEmty()
        {
            showErrorEmtyRadioButton(groupBox2);
            showErrorEmtyComboBox(cbbStoneColor);
            showErrorEmtyTextBox(tbScoopId);
            showErrorEmtyTextBox(tbScoopName);
            showErrorEmtyRadioButton(groupBox1);
            showErrorEmtyRadioButton(groupBox4);
        }


        private void rbWeightIn_Click(object sender, EventArgs e)
        {
            clickWeightIn();
        }

        private void clickWeightIn() 
        {
            ucTruck.BringToFront();
            ucTruck.Show();
            resetMainForm();
            EnableWeightInAndOut();
            getSettingDefault();

            /* สร้างเลข DocNum */
            runningDocNumber();
            /* เช็คเลขที่เอกสาร หากเป็นค่าว่างให้เปิดช่องให้กรอกได้ */
            checkDocNumEmty();
            AfterGetDataFromTable();

            rbWeightIn.Checked = true;

        }

        private void rbWeightOut_Click(object sender, EventArgs e)
        {
            resetMainForm();
            ucTruck.BringToFront();
            ucTruck.Show();

            TableFromDB mf = new TableFromDB(this);
            mf.ShowDialog();
            //ปิดปุ่มอ่านน้ำหนักเข้า
            btReadIn.Enabled = false;
            tbCarLicenseId.Enabled = false;
            tbCarLicense.Enabled = false;
        }

        private void tbDriverId_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void tbDriverName_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbDriverId_Leave(object sender, EventArgs e)
        {
            if (tbDriverId != null && tbDriverId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_driver where รหัสผู้ขับ = '" + tbDriverId.Text + "' and company = '" + Company.Code + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อผู้ขับ"].ToString();
                        tbDriverName.Text = rdStr;
                    }

                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbDriverId.Text = "";
                        tbDriverName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbDriverName.Text = "";
            }
        }

        private void tbDriverName_Leave(object sender, EventArgs e)
        {
            if (tbDriverName != null && tbDriverName.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_driver where ชื่อผู้ขับ = '" + tbDriverName.Text + "'  and company = '" + Company.Code + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["รหัสผู้ขับ"].ToString();
                        tbDriverId.Text = rdStr;
                    }

                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbDriverId.Text = "";
                        tbDriverName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbDriverId.Text = "";
            }
        }

        private void tbCarLicenseId_Leave(object sender, EventArgs e)
        {
            if (tbCarLicenseId != null && tbCarLicenseId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_car_registration where รหัสทะเบียนรถ = '" + tbCarLicenseId.Text + "'  and company = '" + Company.Code + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อทะเบียนรถ"].ToString();
                        tbCarLicense.Text = rdStr;
                    }

                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbCarLicenseId.Text = "";
                        tbCarLicense.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbCarLicense.Text = "";
            }
            //39 ไม่ต้องดึงน้ำหนักเข้า auto getWeightInOnDay(tbCarLicenseId);
        }

        private void tbCarLicense_Leave(object sender, EventArgs e)
        {
            if (tbCarLicense != null && tbCarLicense.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_car_registration where ชื่อทะเบียนรถ = '" + tbCarLicense.Text + "' and company = '" + Company.Code + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["รหัสทะเบียนรถ"].ToString();
                        tbCarLicenseId.Text = rdStr;
                    }

                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbCarLicenseId.Text = "";
                        tbCarLicense.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbCarLicenseId.Text = "";
            }

            //39 ไม่ต้องดึงน้ำหนักเข้า auto getWeightInOnDay(tbCarLicense);
        }

        private void getWeightInOnDay(TextBox tb)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");

            if ((tb != null && tb.Text != "" && checkZeroStr(tbWeightOut.Text)) && tbWeightIn.Enabled)
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT น้ำหนักรถ FROM public.weight where วันที่ = '" + today + "' and ทะเบียนรถ = '" + tbCarLicense.Text + "' ORDER BY weight_id DESC LIMIT 1 ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["น้ำหนักรถ"].ToString();
                        tbWeightIn.Text = tonTokg(rdStr);
                    }
                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbWeightIn.Text = "0.00";
                    }

                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbWeightIn.Text = "0.00";
            }
        }

        private void tbScoopId_Leave(object sender, EventArgs e)
        {
            if (tbScoopId != null && tbScoopId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_scoop where รหัสผู้ตัก = '" + tbScoopId.Text + "' and company = '"+ Company.Code + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อผู้ตัก"].ToString();
                        tbScoopName.Text = rdStr;
                    }

                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbScoopId.Text = "";
                        tbScoopName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbScoopName.Text = "";
            }
        }

        private void tbScoopName_Leave(object sender, EventArgs e)
        {
            if (tbScoopName != null && tbScoopName.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_scoop where ชื่อผู้ตัก = '" + tbScoopName.Text + "' and company = '" + Company.Code + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["รหัสผู้ตัก"].ToString();
                        tbScoopId.Text = rdStr;
                    }
                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbScoopId.Text = "";
                        tbScoopName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbScoopId.Text = "";
            }
        }

        private async void btRefresh_Click(object sender, EventArgs e)
        {
            /* autoComplete ผู้ตัก */
            autoCompleteSettingByCompany(tbScoopId, "รหัสผู้ตัก", "base_scoop");
            autoCompleteSettingByCompany(tbScoopName, "ชื่อผู้ตัก", "base_scoop");

            /* autoComplete ลูกค้า */
            autoCompleteSettingByWeightType(tbCustomerId, "รหัสลูกค้า", "base_customer");
            autoCompleteSettingByWeightType(tbCustomerName, "ชื่อลูกค้า", "base_customer");

            /* autoComplete ผู้ขับ */
            autoCompleteSettingByCompany(tbDriverId, "รหัสผู้ขับ", "base_driver");
            autoCompleteSettingByCompany(tbDriverName, "ชื่อผู้ขับ", "base_driver");

            /* autoComplete ทะเบียนรถ */
            autoCompleteSettingByCompany(tbCarLicenseId, "รหัสทะเบียนรถ", "base_car_registration");
            autoCompleteSettingByCompany(tbCarLicense, "ชื่อทะเบียนรถ", "base_car_registration");


            autoCompleteSettingByWeightType(tbMillId, "รหัสโรงโม่", "base_mill");
            autoCompleteSettingByWeightType(tbMillName, "ชื่อโรงโม่", "base_mill");

            autoCompleteSettingByWeightType(tbSiteId, "base_site_id", "base_site");
            autoCompleteSettingByWeightType(tbSiteName, "base_site_name", "base_site");

            autoCompleteSettingInactive(tbStoneTypeId, "รหัสหิน", "base_stone_type");
            autoCompleteSettingInactive(tbStoneTypeName, "ชื่อหิน", "base_stone_type");

            //fillStoneCombo();
            //fillMillCombo();
            //fillSiteCombo();
        }


        private void getAndCalQ()
        {
            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT ค่าคำนวณคิว FROM public.base_stone_type where ชื่อหิน = '" + tbStoneTypeName.Text + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string rdStr = reader["ค่าคำนวณคิว"].ToString();
                    strCalQ = rdStr;
                }
            }
            catch (Exception)
            {
            }
            dl.close();

            //Weight.StoneColor = getPrintFromDB("base_stone_type", "ประเภทหิน", "ชื่อหิน", cbbStoneType.Text);
            //คำนวณค่าคิว
            calculatenumQ();

        }


        private void tbStoneTypeId_Leave(object sender, EventArgs e)
        {
            if (tbStoneTypeId != null && tbStoneTypeId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT ชื่อหิน FROM public.base_stone_type where รหัสหิน = '" + tbStoneTypeId.Text + "' and inactive = false ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อหิน"].ToString();
                        tbStoneTypeName.Text = rdStr;
                    }
                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbStoneTypeId.Text = "";
                        tbStoneTypeName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbStoneTypeName.Text = "";
            }

            getAndCalQ();
        }

        private void tbStoneTypeName_Leave(object sender, EventArgs e)
        {
            if (tbStoneTypeName != null && tbStoneTypeName.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT รหัสหิน FROM public.base_stone_type where ชื่อหิน = '" + tbStoneTypeName.Text + "' and inactive = false ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["รหัสหิน"].ToString();
                        tbStoneTypeId.Text = rdStr;
                    }
                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbStoneTypeId.Text = "";
                        tbStoneTypeName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbStoneTypeId.Text = "";
            }

            getAndCalQ();
        }

        private void tbMillId_Leave(object sender, EventArgs e)
        {
            if (tbMillId != null && tbMillId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT ชื่อโรงโม่ FROM public.base_mill where รหัสโรงโม่ = '" + tbMillId.Text + "' and (weight_type = 2 or weight_type = 3) ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อโรงโม่"].ToString();
                        tbMillName.Text = rdStr;
                    }
                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbMillId.Text = "";
                        tbMillName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbMillName.Text = "";
            }
        }

        private void tbMillName_Leave(object sender, EventArgs e)
        {
            if (tbMillName != null && tbMillName.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT รหัสโรงโม่ FROM public.base_mill where ชื่อโรงโม่ = '" + tbMillName.Text + "' and (weight_type = 2 or weight_type = 3) ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["รหัสโรงโม่"].ToString();
                        tbMillId.Text = rdStr;
                    }
                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbMillId.Text = "";
                        tbMillName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbMillId.Text = "";
            }
        }

        private void tbSiteId_Leave(object sender, EventArgs e)
        {
            if (tbSiteId != null && tbSiteId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT base_site_name FROM public.base_site where base_site_id = '" + tbSiteId.Text + "' and (weight_type = 2 or weight_type = 3) ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["base_site_name"].ToString();
                        tbSiteName.Text = rdStr;
                    }
                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbSiteId.Text = "";
                        tbSiteName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbSiteName.Text = "";
            }
        }

        private void tbSiteName_Leave(object sender, EventArgs e)
        {
            if (tbSiteName != null && tbSiteName.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT base_site_id FROM public.base_site where base_site_name = '" + tbSiteName.Text + "' and (weight_type = 2 or weight_type = 3) ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["base_site_id"].ToString();
                        tbSiteId.Text = rdStr;
                    }
                    //sql รีเซตค่าหากหาข้อมูลไม่เจอ
                    if (!reader.HasRows)
                    {
                        tbSiteId.Text = "";
                        tbSiteName.Text = "";
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                tbSiteId.Text = "";
            }
        }

        private void tbWeightIn_Leave(object sender, EventArgs e)
        {
            checkNumWeightError(tbWeightIn);
        }

        private void tbWeightOut_Leave(object sender, EventArgs e)
        {
            checkNumWeightError(tbWeightOut);
        }

        private void checkNumWeightError(TextBox tb)
        {
 
            if (tb.Text.Length < 5 && !checkZeroStr(tb.Text))
            {
                MessageBox.Show("ช่อง " + tb.AccessibleName + "มีน้ำหนักน้อยเกินไป กรุณากรอกข้อมูลใหม่", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb.Focus();
            }
            else if (tb.Text.Length == 5)
            {
                char lastNumber = tb.Text[4];
                if (lastNumber != '0')
                {
                    MessageBox.Show("ช่อง " + tb.AccessibleName + "ไม่ได้ลงท้ายด้วย 0 กรุณากรอกข้อมูลใหม่", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tb.Text = "0.00";
                    tb.Focus();
                }
            }
            else if (tb.Text.Length > 5)
            {
                MessageBox.Show("ช่อง " + tb.AccessibleName + "มีน้ำหนักเกิน กรุณากรอกข้อมูลใหม่", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb.Text = "0.00";
                tb.Focus();
            }

        }

    }
}
