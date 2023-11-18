using Devart.Data.PostgreSql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;

namespace SerialPortListener
{
    public partial class TableFromDB : Form
    {
        String usernameDB = null;
        String firstnameDB = null;
        MainForm mainForm;
        Datalayer dl;
        public class DataToUpdate
        {
            public String id;
            public String docNum;
            public String date;
            public String carLicense;
            public String carLicenseId;
            public String carCity;
            public String driverId;
            public String driverName;
            public String customerId;
            public String customerName;
            public String weightIn;
            public String weightOut;
            public String weightTotal;
            public String refNum;
            public String mill;
            public String stoneType;
            public String payType;
            public String scaleId;
            public String scaleName;
            public String scoopId;
            public String scoopName;
            public String pricePerTon;
            public String amount;
            public String shipCost;
            public String weightInDate;
            public String weightInTime;
            public String weightOutDate;
            public String weightOutTime;
            public String q;
            public String approveId;
            public String approveName;
            public String amountVat;
            public String vatType;
            public String stoneColor;
            public String site;
            public String team;
            public String clean;
            public String transport;
            public String note;
            public String siteId;
            public String stoneTypeId;
            public String millId;
        }
        public TableFromDB(MainForm parent)
        {
            dl = new Datalayer();
            InitializeComponent();
            mainForm = parent;
            cbbSearchWeight.SelectedIndex = 1;
            setDefaultFormatDTGV();
        }

        private void setDefaultFormatDTGV() {
            tableDataFromDB.Columns["ราคาตัน"].DefaultCellStyle.Format = "n2";
            tableDataFromDB.Columns["จำนวณเงิน"].DefaultCellStyle.Format = "n2";
            tableDataFromDB.Columns["จำนวนเงินสุทธิ"].DefaultCellStyle.Format = "n2";
            tableDataFromDB.Columns["vat"].DefaultCellStyle.Format = "n2";
            tableDataFromDB.Columns["คิว"].DefaultCellStyle.Format = "n2";

            tableDataFromDB.Columns["น้ำหนักรถ"].DefaultCellStyle.Format = "n3";
            tableDataFromDB.Columns["น้ำหนักรวม"].DefaultCellStyle.Format = "n3";
            tableDataFromDB.Columns["น้ำหนักสินค้า"].DefaultCellStyle.Format = "n3";
        }

        private void setDefaultFromDB(String username, String firstname) {
            usernameDB = username;
            firstnameDB = firstname;
        }
        private void TableFromDB_Load(object sender, EventArgs e)
        {
            try
            {
                // TODO: This line of code loads data into the 'truckDataSet.weight' table. You can move, or remove it, as needed.
                setSearchDateFromTo();
            }
            catch (Exception ex) { 
            }

        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            
            //set Mode Weight
            //Globals.ModeWeight = "A";

            mainForm.resetMainForm();
            mainForm.EnableWeightInAndOut();
            mainForm.AfterGetDataFromTable();
            mainForm.getSettingDefault();

            /* สร้างเลข DocNum */
            mainForm.runningDocNumber();
            /* เช็คเลขที่เอกสาร หากเป็นค่าว่างให้เปิดช่องให้กรอกได้ */
            mainForm.checkDocNumEmty();
            //System.Threading.Thread.Sleep(100);
            this.Hide();

        }

        private void btDelete_Click(object sender, EventArgs e)
        {            
            string selectedId = tableDataFromDB.CurrentRow.Cells["weight_id"].Value.ToString();
            string selectedDocNum = tableDataFromDB.CurrentRow.Cells["เลขที่เอกสาร"].Value.ToString();

            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "DELETE FROM weight WHERE weight_id = '" + selectedId + "' AND เลขที่เอกสาร = '" + selectedDocNum + "'";
            try
            {
                DialogResult dialog = MessageBox.Show("ต้องการลบเลขที่เอกสาร "+ selectedDocNum + " ใช่หรือไม่","ลบรายการ",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    int rowIndex = tableDataFromDB.CurrentCell.RowIndex;
                    tableDataFromDB.Rows.RemoveAt(rowIndex);
                }
            }
            catch (Exception)
            {

            }
            dl.close();

        }

        private void btClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void prepareDataToMainForm() {
            mainForm.resetMainForm();
            DataToUpdate data = new DataToUpdate();
            if (tableDataFromDB.Rows.Count > 1) {
                data.id = tableDataFromDB.CurrentRow.Cells["weight_id"].Value.ToString();
                data.docNum = tableDataFromDB.CurrentRow.Cells["เลขที่เอกสาร"].Value.ToString();
                data.amount = tableDataFromDB.CurrentRow.Cells["จำนวณเงิน"].Value.ToString();
                data.carCity = tableDataFromDB.CurrentRow.Cells["จังหวัด"].Value.ToString();
                data.carLicense = tableDataFromDB.CurrentRow.Cells["ทะเบียนรถ"].Value.ToString();
                data.customerId = tableDataFromDB.CurrentRow.Cells["รหัสลูกค้า"].Value.ToString();
                data.customerName = tableDataFromDB.CurrentRow.Cells["ลูกค้า"].Value.ToString();
                data.date = tableDataFromDB.CurrentRow.Cells["วันที่"].Value.ToString();
                data.docNum = tableDataFromDB.CurrentRow.Cells["เลขที่เอกสาร"].Value.ToString();
                data.driverName = tableDataFromDB.CurrentRow.Cells["คนขับ"].Value.ToString();
                data.mill = tableDataFromDB.CurrentRow.Cells["โรงโม่"].Value.ToString();
                data.payType = tableDataFromDB.CurrentRow.Cells["จ่ายเงิน"].Value.ToString();
                data.pricePerTon = tableDataFromDB.CurrentRow.Cells["ราคาตัน"].Value.ToString();
                data.refNum = tableDataFromDB.CurrentRow.Cells["เลขที่ใบตัก"].Value.ToString();
                data.scaleId = tableDataFromDB.CurrentRow.Cells["รหัสผู้ชั่ง"].Value.ToString();
                data.scaleName = tableDataFromDB.CurrentRow.Cells["ชื่อผู้ชั่ง"].Value.ToString();
                data.scoopId = tableDataFromDB.CurrentRow.Cells["รหัสผู้ตัก"].Value.ToString();
                data.scoopName = tableDataFromDB.CurrentRow.Cells["ชื่อผู้ตัก"].Value.ToString();
                data.shipCost = tableDataFromDB.CurrentRow.Cells["ค่าขนส่ง"].Value.ToString();
                data.stoneType = tableDataFromDB.CurrentRow.Cells["ชนิดหิน"].Value.ToString();
                data.weightIn = tableDataFromDB.CurrentRow.Cells["น้ำหนักรถ"].Value.ToString();
                data.weightInDate = tableDataFromDB.CurrentRow.Cells["วันที่ชั่งเข้า"].Value.ToString();
                data.weightInTime = tableDataFromDB.CurrentRow.Cells["เวลาชั่งเข้า"].Value.ToString();
                data.weightOut = tableDataFromDB.CurrentRow.Cells["น้ำหนักรวม"].Value.ToString();
                data.weightOutDate = tableDataFromDB.CurrentRow.Cells["วันที่ชั่งออก"].Value.ToString();
                data.weightOutTime = tableDataFromDB.CurrentRow.Cells["เวลาชั่งออก"].Value.ToString();
                data.weightTotal = tableDataFromDB.CurrentRow.Cells["น้ำหนักสินค้า"].Value.ToString();
                data.q = tableDataFromDB.CurrentRow.Cells["คิว"].Value.ToString();
                data.approveId = tableDataFromDB.CurrentRow.Cells["รหัสผู้อนุมัติจ่าย"].Value.ToString();
                data.approveName = tableDataFromDB.CurrentRow.Cells["ชื่อผู้อนุมัติจ่าย"].Value.ToString();
                data.amountVat = tableDataFromDB.CurrentRow.Cells["จำนวนเงินสุทธิ"].Value.ToString();
                data.vatType = tableDataFromDB.CurrentRow.Cells["ชนิดvat"].Value.ToString();
                data.stoneColor = tableDataFromDB.CurrentRow.Cells["ประเภทหิน"].Value.ToString();
                data.site = tableDataFromDB.CurrentRow.Cells["หน้างาน"].Value.ToString();
                data.team = tableDataFromDB.CurrentRow.Cells["ทีม"].Value.ToString();
                data.clean = tableDataFromDB.CurrentRow.Cells["ล้าง"].Value.ToString();
                data.transport = tableDataFromDB.CurrentRow.Cells["ขนส่ง"].Value.ToString();
                data.driverId = tableDataFromDB.CurrentRow.Cells["รหัสคนขับ"].Value.ToString();
                data.carLicenseId = tableDataFromDB.CurrentRow.Cells["รหัสทะเบียนรถ"].Value.ToString();
                data.note = tableDataFromDB.CurrentRow.Cells["หมายเหตุ"].Value.ToString();

                data.siteId = tableDataFromDB.CurrentRow.Cells["site_id"].Value.ToString();
                data.millId = tableDataFromDB.CurrentRow.Cells["mill_id"].Value.ToString();
                data.stoneTypeId = tableDataFromDB.CurrentRow.Cells["stone_type_id"].Value.ToString();

                //set Mode Weight
                /*
                if (data.weightOut == "0.000")
                    Globals.ModeWeight = "U";
                else
                    Globals.ModeWeight = "F";
                */

                mainForm.setDataFromClassTableFromDB(data);
                //System.Threading.Thread.Sleep(100);
                this.Hide();
            }
            else {
                MessageBox.Show("ไม่พบข้อมูลที่ต้องการแก้ไข", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btUpdate_Click(object sender, EventArgs e)
        {
            //mainForm.disableReadWeightIn();
            mainForm.getSettingDefault();
            prepareDataToMainForm();
        }

        private void tableDataFromDB_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //mainForm.disableReadWeightIn();
            mainForm.getSettingDefault();
            prepareDataToMainForm();
        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            setSearchDateFromTo();
        }

        private void setSearchDateFromTo()
        {
            /*
            //sql find
            StringBuilder sql = new StringBuilder();
            sql.Append("วันที่ >= '" + dateFrom.Text + "' AND  วันที่ <= '" + dateTo.Text + "'");
            if (cbbSearchWeight.SelectedIndex == 1)
            {
                sql.Append(" AND น้ำหนักรวม = '0.00'");
                //this.tableDataFromDB.Sort(this.tableDataFromDB.Columns["น้ำหนักรถ"], ListSortDirection.Ascending);
            }
            else { 
            
            }
            this.weightBindingSource.Filter = sql.ToString();
            */
            try
            {
                dl.connect();
                //string sql = "SELECT * FROM public.weight where วันที่ between '" + dateFrom.Text + "'  and  '" + dateTo.Text + "'";
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM public.weight where วันที่ between '").Append(dateFrom.Value.ToString("yyyy-MM-dd")).Append("'  AND  '").Append(dateTo.Value.ToString("yyyy-MM-dd")).Append("'");
                if (cbbSearchWeight.SelectedIndex == 1)
                    sql.Append(" AND น้ำหนักรวม = '0.00' AND NOT รหัสลูกค้า = '99' AND NOT รหัสลูกค้า = '99RM' ");
                else if (cbbSearchWeight.SelectedIndex == 2)
                    sql.Append(" AND NOT น้ำหนักรถ = '0.00' AND NOT น้ำหนักรวม = '0.00' ");

                sql.Append(" ORDER BY วันที่, เลขที่เอกสาร");
                OdbcDataAdapter cmd = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                DataTable dt = new DataTable();
                cmd.Fill(dt);
                tableDataFromDB.DataSource = dt;
            }
            catch (Exception)
            {
            }
            dl.close();

        }

    }
}
