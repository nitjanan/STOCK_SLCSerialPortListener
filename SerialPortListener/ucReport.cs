using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Devart.Data.PostgreSql;
using System.Data.Odbc;

namespace SerialPortListener
{
    public partial class ucReport : UserControl
    {
        Datalayer dl;

        private static ucReport _instance;
        private static ucReport Instance
        {

            get
            {
                if (_instance == null)
                    _instance = new ucReport();
                return _instance;
            }
        }
        public ucReport()
        {
            dl = new Datalayer();
            InitializeComponent();
            setDefaultFormatDTGV();
            /* autoComplete ลูกค้า */
            autoCompleteSetting(tbCustomerId, "รหัสลูกค้า", "base_customer");
            autoCompleteSetting(tbCustomerName, "ชื่อลูกค้า", "base_customer");
            /* autoComplete แจ้งหนี้ */
            autoCompleteSetting(tbInvoiceCutomerId, "รหัสลูกค้า", "base_customer");
            autoCompleteSetting(tbInvoiceCutomerName, "ชื่อลูกค้า", "base_customer");

            /* autoComplete ทะเบียนรถ */
            autoCompleteSettingDistinct(tbJointCarRegistration, "ทะเบียนรถ", "weight");

            /* autoComplete ชื่อผู้ตัก */
            autoCompleteSettingDistinct(tbScoopName, "ชื่อผู้ตัก", "weight");

            cbMill.SelectedIndex = 0;

            //tabPage1
            cbbMill.SelectedIndex = 0;
            cbbCarType.SelectedIndex = 0;
            cbbWeight.SelectedIndex = 0;

            fillTableCombo(cbbCarRegistration, "base_car_registration", "ชื่อทะเบียนรถ");
            fillTableCombo(cbbDriver, "base_driver", "ชื่อผู้ขับ");
            fillTableCombo(cbbStoneType, "base_stone_type", "ชื่อหิน");
            fillTableCombo(cbbCustomer, "base_customer", "ชื่อลูกค้า");
            fillTableCombo(cbbCustomerSummary, "base_customer", "ชื่อลูกค้า");
            fillTableCombo(cbbDriverSummary, "base_driver", "ชื่อผู้ขับ");
            fillTableCombo(cbbStone, "base_stone_type", "ชื่อหิน");

            dtFromOut.Value = Convert.ToDateTime(System.DateTime.Today.ToShortDateString() + " 08:00 AM");

        }

        private void setDefaultFormatDTGV() {
            dgvDailyReport.Columns["ราคาตัน"].DefaultCellStyle.Format = "n2";
            dgvDailyReport.Columns["จำนวนเงิน"].DefaultCellStyle.Format = "n2";
            dgvDailyReport.Columns["จำนวนเงินสุทธิ"].DefaultCellStyle.Format = "n2";
            dgvDailyReport.Columns["vat"].DefaultCellStyle.Format = "n2";
            dgvDailyReport.Columns["คิว"].DefaultCellStyle.Format = "n2";

            dgvDailyReport.Columns["น้ำหนักรถ"].DefaultCellStyle.Format = "n3";
            dgvDailyReport.Columns["น้ำหนักรวม"].DefaultCellStyle.Format = "n3";
            dgvDailyReport.Columns["น้ำหนักสินค้า"].DefaultCellStyle.Format = "n3";
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
            catch (Exception)
            {
            }
            tb.AutoCompleteCustomSource = coll;
            dl.close();
        }

        /* autoComplete Setting  แบบข้อมูลไม่ซ้ำกัน */
        private void autoCompleteSettingDistinct(TextBox tb, string field, string tableName)
        {
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
            AutoCompleteStringCollection coll = new AutoCompleteStringCollection();

            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT DISTINCT "+ field + " FROM public." + tableName;
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

        private void setDataWeightToDTGV()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT weight.*  FROM public.weight");
                if (cbbCarType.SelectedIndex != 0)
                    sql.Append(" INNER JOIN public.base_car_registration ON weight.ทะเบียนรถ = base_car_registration.ชื่อทะเบียนรถ ");
                sql.Append(" WHERE วันที่ >= '" + tbdateFrom.Value.ToString("yyyy-MM-dd") + "' AND  วันที่ <= '" + tbdateTo.Value.ToString("yyyy-MM-dd") + "' AND เวลาชั่งออก BETWEEN '" + dtFromOut.Value.ToString("HH:mm") + "' AND '" + dtToOut.Value.ToString("HH:mm") + "' ");
                if (cbbCarType.SelectedIndex != 0)
                    sql.Append(" AND base_car_registration.ประเภทรถ = '" + cbbCarType.Text + "' ");
                if (cbbCarRegistration.SelectedIndex != 0)
                    sql.Append(" AND ทะเบียนรถ = '" + cbbCarRegistration.Text + "' ");
                if (cbbMill.SelectedIndex != 0)
                    sql.Append(" AND โรงโม่ = '" + cbbMill.Text + "' ");
                if (cbbDriver.SelectedIndex != 0)
                    sql.Append(" AND คนขับ = '" + cbbDriver.Text + "' ");
                if (cbbStoneType.SelectedIndex != 0)
                    sql.Append(" AND ชนิดหิน = '" + cbbStoneType.Text + "' ");
                if (cbbCustomer.SelectedIndex != 0)
                    sql.Append(" AND ลูกค้า = '" + cbbCustomer.Text + "' ");
                if (cbbWeight.SelectedIndex != 0)
                    sql.Append(" AND น้ำหนักสินค้า " + checkWeightStr(cbbWeight.Text) + "");
                sql.Append(" ORDER BY วันที่, เลขที่เอกสาร ");

                OdbcDataAdapter adt = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                DataTable dt = new DataTable();
                adt.Fill(dt);
                dgvDailyReport.DataSource = dt;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private string checkWeightStr(string strWeight) {
            string weight = "";
            switch (strWeight)
            {
                case "น้อยกว่า 14":
                    weight = "< 14";
                    break;
                case "มากกว่าหรือเท่ากับ 14":
                    weight = ">= 14";
                    break;
                case "น้อยกว่า 20":
                    weight = "< 20";
                    break;
                case "มากกว่าหรือเท่ากับ 20":
                    weight = ">= 20";
                    break;
                case "น้อยกว่า 28":
                    weight = "< 28";
                    break;
                case "มากกว่าหรือเท่ากับ 28":
                    weight = ">= 28";
                    break;
                case "น้อยกว่า 40":
                    weight = "< 40";
                    break;
                case "มากกว่าหรือเท่ากับ 40":
                    weight = ">= 40";
                    break;
            }

            return weight;
        } 

        private void ucReport_Load(object sender, EventArgs e)
        {
            //this.weightTableAdapter.Fill(this.truckDataSet.weight);
            //this.weightBindingSource.Filter = string.Format("วันที่ = '" + tbdateTo.Text + "' AND ( NOT น้ำหนักรวม = 0.00 OR  รหัสลูกค้า = '999' ) ");
            setDataWeightToDTGV();
        }

        private void tcReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcReport.SelectedTab == tabPage1)
            {
                cbbMill.SelectedIndex = 0;
                fillTableCombo(cbbCarRegistration, "base_car_registration", "ชื่อทะเบียนรถ");
                fillTableCombo(cbbDriver, "base_driver", "ชื่อผู้ขับ");
                fillTableCombo(cbbStoneType, "base_stone_type", "ชื่อหิน");
            }
            else if (tcReport.SelectedTab == tabPage6)
            {
                fillCarTeamCombo(cbbCarTeamName);
            }
            else if (tcReport.SelectedTab == tabPage12)
            {
                fillCarTeamCombo(cbbCarTeamNameByTeam);
            }
        }

        private void fillCarTeamCombo(ComboBox cbb)
        {
            //ล้างก่อน
            cbb.Items.Clear();

            //
            cbb.Items.Add("ทั้งหมด");
            cbb.SelectedIndex = 0;

            //เพิ่ม combobox
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public.base_car_team";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string des = reader["ชื่อทีม"].ToString();
                    cbb.Items.Add(des);
                }
            }
            catch (Exception)
            {

            }
            dl.close();
        }

        private void fillTableCombo(ComboBox cbb, string tableName, string field)
        {
            //ล้างก่อน
            cbb.Items.Clear();

            //
            cbb.Items.Add("ทั้งหมด");
            cbb.SelectedIndex = 0;

            //เพิ่ม combobox
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT * FROM public."+ tableName;
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string des = reader[field].ToString();
                    cbb.Items.Add(des);
                }
            }
            catch (Exception)
            {

            }
            dl.close();
        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            /*
            this.weightTableAdapter.Fill(this.truckDataSet.weight);
            StringBuilder sql = new StringBuilder();
            sql.Append("วันที่ >= '" + tbdateFrom.Text + "' AND  วันที่ <= '" + tbdateTo.Text + "' AND ( NOT น้ำหนักรวม = 0.00 OR  รหัสลูกค้า = '999' ) ");
            this.weightBindingSource.Filter = sql.ToString();
            */
            setDataWeightToDTGV();
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            /*  print แบบเก่า
             
            Microsoft.Reporting.WinForms.ReportDataSource rs = new Microsoft.Reporting.WinForms.ReportDataSource();
            double sumTotal = 0;
            double sumQ = 0;
            double sumAmount = 0;
            double sumAmountVat = 0;
            List<WeightDailyReport> listWDR = new List<WeightDailyReport>();
            listWDR.Clear();

            for (int i = 0; i < dgvDailyReport.Rows.Count - 1; i++) {
                listWDR.Add(new WeightDailyReport()
                {
                    วันที่ = dateFormatToPrint(dgvDailyReport.Rows[i].Cells["วันที่"].Value.ToString()),
                    เลขที่เอกสาร = dgvDailyReport.Rows[i].Cells["เลขที่เอกสาร"].Value.ToString(),
                    รหัสลูกค้า = dgvDailyReport.Rows[i].Cells["รหัสลูกค้า"].Value.ToString(),
                    ลูกค้า = dgvDailyReport.Rows[i].Cells["ลูกค้า"].Value.ToString(),
                    ทะเบียนรถ = dgvDailyReport.Rows[i].Cells["ทะเบียนรถ"].Value.ToString(),
                    จังหวัด = dgvDailyReport.Rows[i].Cells["จังหวัด"].Value.ToString(),
                    ทีม = dgvDailyReport.Rows[i].Cells["ทีม"].Value.ToString(),
                    ชนิดหิน = dgvDailyReport.Rows[i].Cells["ชนิดหิน"].Value.ToString(),
                    ราคาตัน = dgvDailyReport.Rows[i].Cells["ราคาตัน"].Value.ToString(),
                    น้ำหนักสินค้า = dgvDailyReport.Rows[i].Cells["น้ำหนักสินค้า"].Value.ToString(),
                    คิว = dgvDailyReport.Rows[i].Cells["คิว"].Value.ToString(),
                    จำนวนเงิน = dgvDailyReport.Rows[i].Cells["จำนวนเงิน"].Value.ToString(),
                    จำนวนเงินสุทธิ = dgvDailyReport.Rows[i].Cells["จำนวนเงินสุทธิ"].Value.ToString(),
                    จ่ายเงิน = dgvDailyReport.Rows[i].Cells["จ่ายเงิน"].Value.ToString(),
                    ชื่อผู้ตัก = dgvDailyReport.Rows[i].Cells["ชื่อผู้ตัก"].Value.ToString(),
                });
                sumTotal += Convert.ToDouble(dgvDailyReport.Rows[i].Cells["น้ำหนักสินค้า"].Value);
                sumQ += Convert.ToDouble(dgvDailyReport.Rows[i].Cells["คิว"].Value);
                sumAmount += Convert.ToDouble(dgvDailyReport.Rows[i].Cells["จำนวนเงิน"].Value);
                sumAmountVat += Convert.ToDouble(dgvDailyReport.Rows[i].Cells["จำนวนเงินสุทธิ"].Value);
            }
            WeightDailyReport.DateFrom = tbdateFrom.Text;
            WeightDailyReport.DateTo = tbdateTo.Text;
            WeightDailyReport.SumToTal = sumTotal.ToString("#,##0.00");
            WeightDailyReport.SumQ = sumQ.ToString("#,##0.00");
            WeightDailyReport.SumAmount = sumAmount.ToString("#,##0.00");
            WeightDailyReport.SumAmountVat = sumAmountVat.ToString("#,##0.00");
            WeightDailyReport.CountRount = (dgvDailyReport.Rows.Count-1).ToString();
            rs.Name = "DailyDataSet";
            rs.Value = listWDR;
            FPrintDailyReport fp = new FPrintDailyReport(listWDR, rs);
            fp.ShowDialog();

            */

            //sql
            dl.connect();
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT weight.*  FROM public.weight");
            if (cbbCarType.SelectedIndex != 0)
                sql.Append(" INNER JOIN public.base_car_registration ON weight.ทะเบียนรถ = base_car_registration.ชื่อทะเบียนรถ ");
            sql.Append(" WHERE วันที่ >= '" + tbdateFrom.Value.ToString("yyyy-MM-dd") + "' AND  วันที่ <= '" + tbdateTo.Value.ToString("yyyy-MM-dd") + "' AND เวลาชั่งออก BETWEEN '" + dtFromOut.Value.ToString("HH:mm") + "' AND '" + dtToOut.Value.ToString("HH:mm") + "' ");
            if (cbbCarType.SelectedIndex != 0)
                sql.Append(" AND base_car_registration.ประเภทรถ = '" + cbbCarType.Text + "' ");
            if (cbbCarRegistration.SelectedIndex != 0)
                sql.Append(" AND ทะเบียนรถ = '" + cbbCarRegistration.Text + "' ");
            if (cbbMill.SelectedIndex != 0)
                sql.Append(" AND โรงโม่ = '" + cbbMill.Text + "' ");
            if (cbbDriver.SelectedIndex != 0)
                sql.Append(" AND คนขับ = '" + cbbDriver.Text + "' ");
            if (cbbStoneType.SelectedIndex != 0)
                sql.Append(" AND ชนิดหิน = '" + cbbStoneType.Text + "' ");
            if (cbbCustomer.SelectedIndex != 0 )
                sql.Append(" AND ลูกค้า = '" + cbbCustomer.Text + "' ");
            if (cbbWeight.SelectedIndex != 0)
                sql.Append(" AND น้ำหนักสินค้า " + checkWeightStr(cbbWeight.Text) + "");
            sql.Append(" ORDER BY วันที่, เลขที่เอกสาร ");

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightDailyReport.DateFrom = tbdateFrom.Text;
            WeightDailyReport.DateTo = tbdateTo.Text;

            //open winform
            List<WeightDailyReport> listWDR = new List<WeightDailyReport>();
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("dailyDataSet", dt);
            FPrintDailyReport fp = new FPrintDailyReport(listWDR, rds);
            fp.ShowDialog();

        }

        private string dateFormatToPrint(string strDate) {
            DateTime date = Convert.ToDateTime(strDate);
            string dateFormat = date.ToString("dd/MM/yyyy");
            return dateFormat;
        }

        private void btExport_Click(object sender, EventArgs e)
        {
            double sumTotal = 0;
            double sumQ = 0;
            double sumAmount = 0;
            double sumAmountVat = 0;
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            worksheet.Name = "DailyReportSheet";

            worksheet.Cells[1, 1] = "รายงานการชั่งสินค้าประจำวันที่ "+ tbdateFrom.Text + " - "+ tbdateTo.Text;
            for (int i = 1; i< dgvDailyReport.Columns.Count + 1; i++) {
                worksheet.Cells[3,i] = dgvDailyReport.Columns[i - 1].HeaderText;
            }

            for (int i = 0; i < dgvDailyReport.Rows.Count; i++) {
                sumTotal += Convert.ToDouble(dgvDailyReport.Rows[i].Cells["น้ำหนักสินค้า"].Value);
                sumQ += Convert.ToDouble(dgvDailyReport.Rows[i].Cells["คิว"].Value);
                sumAmount += Convert.ToDouble(dgvDailyReport.Rows[i].Cells["จำนวนเงิน"].Value);
                sumAmountVat += Convert.ToDouble(dgvDailyReport.Rows[i].Cells["จำนวนเงินสุทธิ"].Value);

                for (int j = 0; j < dgvDailyReport.Columns.Count; j++) {
                    string tmpStr = null;
                    if (dgvDailyReport.Rows[i].Cells[j].Value == null)
                        tmpStr = "";
                    else if (dgvDailyReport.Rows[i].Cells[j].ValueType.ToString().Equals("System.DateTime"))
                        tmpStr = dateFormatToPrint(dgvDailyReport.Rows[i].Cells[j].Value.ToString());
                    else
                        tmpStr = dgvDailyReport.Rows[i].Cells[j].Value.ToString();
                    worksheet.Cells[i + 4, j + 1] = tmpStr;
                }
            }
            WeightDailyReport.SumToTal = sumTotal.ToString("#,##0.00");
            WeightDailyReport.SumQ = sumQ.ToString("#,##0.00");
            WeightDailyReport.SumAmount = sumAmount.ToString("#,##0.00");
            WeightDailyReport.SumAmountVat = sumAmountVat.ToString("#,##0.00");

            int coutRows = dgvDailyReport.Rows.Count+4;
            worksheet.Cells[coutRows, 1] = "รวม";
            worksheet.Cells[coutRows, 10] = WeightDailyReport.SumToTal;
            worksheet.Cells[coutRows, 11] = WeightDailyReport.SumQ;
            worksheet.Cells[coutRows, 12] = WeightDailyReport.SumAmount;
            worksheet.Cells[coutRows, 13] = WeightDailyReport.SumAmountVat;

            worksheet.Cells[coutRows + 1, 1] = "จำนวนเที่ยว";
            worksheet.Cells[coutRows + 1, 2] = (dgvDailyReport.Rows.Count - 1).ToString();
            worksheet.Cells[coutRows + 1, 3] = "เที่ยว";

            var saveFileDialoge = new SaveFileDialog();
            saveFileDialoge.FileName = "DailyReport";
            saveFileDialoge.DefaultExt = ".xlsx";
            if (saveFileDialoge.ShowDialog() == DialogResult.OK) {
                workbook.SaveAs(saveFileDialoge.FileName,Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            app.Quit();

        }

        private void btPrintScoop_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromScoop.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToScoop.Value.ToString("yyyy-MM-dd") + "'";
            if (tbScoopName.Text != "")
                sql += " AND ชื่อผู้ตัก LIKE '%" + tbScoopName.Text + "%' ";
            sql += " ORDER BY วันที่, เลขที่เอกสาร ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromScoop.Text;
            WeightTempReport.DateTo = dtToScoop.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("scoopDataSet", dt);
            FPrintScoopReport fp = new FPrintScoopReport(rds);
            fp.ShowDialog();
        }

        private void btPrintJointCar_Click(object sender, EventArgs e)
        {

            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromJointCar.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToJointCar.Value.ToString("yyyy-MM-dd") + "'";
            if (tbJointCarRegistration.Text != "")
                sql += " AND ทะเบียนรถ LIKE '%" + tbJointCarRegistration.Text + "%' ";
            sql += " ORDER BY วันที่, เลขที่เอกสาร ";
            
            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromJointCar.Text;
            WeightTempReport.DateTo = dtToJointCar.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("jointCarDataSet", dt);
            FPrintJointCarReport fp = new FPrintJointCarReport(rds);
            fp.ShowDialog();
            
        }

        private void btPrintCarTeam_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "SELECT base_car.รหัสรถร่วม, base_car.ชื่อรถร่วม, base_car.รหัสทีม, base_car_team.ชื่อทีม FROM base_car INNER JOIN base_car_team ON base_car.รหัสทีม = base_car_team.รหัสทีม ";
            if (cbbCarTeamName.SelectedIndex != 0)
                sql += "WHERE base_car_team.ชื่อทีม = '" + cbbCarTeamName.Text + "'";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();


            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("carTeamDataSet", dt);
            FPrintCarTeamReport fp = new FPrintCarTeamReport(rds);
            fp.ShowDialog();
        }


        private void btPrintInvoice_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromInvoice.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToInvoice.Value.ToString("yyyy-MM-dd") + "'";
            if (tbInvoiceCutomerId.Text != "")
                sql += " AND รหัสลูกค้า = '" + tbInvoiceCutomerId.Text + "' ";
            sql += " ORDER BY วันที่, เลขที่เอกสาร ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromInvoice.Text;
            WeightTempReport.DateTo = dtToInvoice.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("invoiceDataSet", dt);
            FPrintInvoiceReport fp = new FPrintInvoiceReport(rds);
            fp.ShowDialog();
        }

        private void btPrintCustomer_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromCustomer.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToCustomer.Value.ToString("yyyy-MM-dd") + "' ";
            if (tbCustomerId.Text != "")
                sql += " AND รหัสลูกค้า = '" + tbCustomerId.Text + "' ";
            sql += " ORDER BY วันที่ ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromCustomer.Text;
            WeightTempReport.DateTo = dtToCustomer.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("customerDataSet", dt);
            FPrintCustomerReport fp = new FPrintCustomerReport(rds);
            fp.ShowDialog();
        }

        private void setCustomerIdToTextbox(TextBox customerId, TextBox customernName){

            if (customerId != null && customerId.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_customer where รหัสลูกค้า = '" + customerId.Text + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["ชื่อลูกค้า"].ToString();

                        customernName.Text = rdStr;
                    }


                }
                catch (Exception)
                {
                }

                dl.close();
            }
            else
            {
                customernName.Text = "";
            }
        }

        private void tbCustomerId_TextChanged(object sender, EventArgs e)
        {
            setCustomerIdToTextbox(tbCustomerId, tbCustomerName);
        }

        private void setCustomerNameToTextbox(TextBox customerId, TextBox customernName)
        {
            if (customernName != null && customernName.Text != "")
            {
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT * FROM public.base_customer where ชื่อลูกค้า = '" + customernName.Text + "' ";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        string rdStr = reader["รหัสลูกค้า"].ToString();
                        customerId.Text = rdStr;
                    }
                }
                catch (Exception)
                {
                }
                dl.close();
            }
            else
            {
                customerId.Text = "";
            }
        }
        private void tbCustomerName_TextChanged(object sender, EventArgs e)
        {
            setCustomerNameToTextbox(tbCustomerId, tbCustomerName);
        }

        private void btPrintClean_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromClean.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToClean.Value.ToString("yyyy-MM-dd") + "' AND ล้าง = '" + getRadioValueClean() + "' ORDER BY วันที่, เลขที่เอกสาร ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromClean.Text;
            WeightTempReport.DateTo = dtToClean.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("cleanDataSet", dt);
            FPrintCleanReport fp = new FPrintCleanReport(rds);
            fp.ShowDialog();
        }

        private String getRadioValueClean() {
            string value = "";
            if (rbCleanStone.Checked)
                value = rbCleanStone.Text;
            else if (rbCleanWater.Checked)
                value = rbCleanWater.Text;
            else if (rbCleanNo.Checked)
                value = rbCleanNo.Text;
            return value;
        }

        private void btPrintMill_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromMill.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToMill.Value.ToString("yyyy-MM-dd") + "' AND โรงโม่ = '" + cbMill.Text + "' ORDER BY วันที่, เลขที่เอกสาร ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromMill.Text;
            WeightTempReport.DateTo = dtToMill.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("millDataSet", dt);
            FPrintMillReport fp = new FPrintMillReport(rds);
            fp.ShowDialog();
        }

        private void btPrintSite_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromSite.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToSite.Value.ToString("yyyy-MM-dd") + "' ORDER BY วันที่, เลขที่เอกสาร ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromSite.Text;
            WeightTempReport.DateTo = dtToSite.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("siteDataSet", dt);
            FPrintSiteReport fp = new FPrintSiteReport(rds);
            fp.ShowDialog();
        }

        private void tbInvoiceCutomerId_TextChanged(object sender, EventArgs e)
        {
            setCustomerIdToTextbox(tbInvoiceCutomerId, tbInvoiceCutomerName);
        }

        private void tbInvoiceCutomerName_TextChanged(object sender, EventArgs e)
        {
            setCustomerNameToTextbox(tbInvoiceCutomerId, tbInvoiceCutomerName);
        }

        private void btPrintStoneType_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromStoneType.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToStoneType.Value.ToString("yyyy-MM-dd") + "' AND NOT น้ำหนักรวม = 0.00 AND NOT โรงโม่ = ' ' ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromStoneType.Text;
            WeightTempReport.DateTo = dtToStoneType.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("stoneTypeDataSet", dt);
            FPrintStoneTypeReport fp = new FPrintStoneTypeReport(rds);
            fp.ShowDialog();
        }

        private void btPrintTransport_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromTransport.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToTransport.Value.ToString("yyyy-MM-dd") + "' ORDER BY วันที่, เลขที่เอกสาร ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromTransport.Text;
            WeightTempReport.DateTo = dtToTransport.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("transportDataSet", dt);
            FPrintTransportReport fp = new FPrintTransportReport(rds);
            fp.ShowDialog();

        }

        private void btPrintTransportByTeam_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            string sql = "select * from weight where วันที่ BETWEEN '" + dtFromTransportByTeam.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToTransportByTeam.Value.ToString("yyyy-MM-dd") + "' AND NOT รหัสลูกค้า = '999' ";
            if (cbbCarTeamNameByTeam.SelectedIndex != 0)
                sql += " AND ทีม = '" + cbbCarTeamNameByTeam.Text + "'";
            sql += " ORDER BY วันที่, เลขที่เอกสาร ";

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql, dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromTransportByTeam.Text;
            WeightTempReport.DateTo = dtToTransportByTeam.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("transportByTeamDataSet", dt);
            FPrintTransportByTeamReport fp = new FPrintTransportByTeamReport(rds);
            fp.ShowDialog();
        }

        private string strNotEmty(string str)
        {
            return str == "" ? " " : str;
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

        private void btPrintBil_Click(object sender, EventArgs e)
        {
            if (dgvDailyReport.Rows.Count > 1)
            {
                Weight.DocNum = dgvDailyReport.CurrentRow.Cells["เลขที่เอกสาร"].Value.ToString();
                Weight.Amount = numberFormat(dgvDailyReport.CurrentRow.Cells["จำนวนเงิน"].Value.ToString(), 2);
                Weight.CarCity = strNotEmty(dgvDailyReport.CurrentRow.Cells["จังหวัด"].Value.ToString());
                Weight.CarLicense = strNotEmty(dgvDailyReport.CurrentRow.Cells["ทะเบียนรถ"].Value.ToString());
                Weight.CustomerName = strNotEmty(dgvDailyReport.CurrentRow.Cells["ลูกค้า"].Value.ToString());
                Weight.Date = dgvDailyReport.CurrentRow.Cells["วันที่"].Value.ToString();
                Weight.DriverName = strNotEmty(dgvDailyReport.CurrentRow.Cells["คนขับ"].Value.ToString());
                Weight.Mill = strNotEmty(dgvDailyReport.CurrentRow.Cells["โรงโม่"].Value.ToString());
                Weight.Pay = dgvDailyReport.CurrentRow.Cells["จ่ายเงิน"].Value.ToString();
                Weight.Price = numberFormat(dgvDailyReport.CurrentRow.Cells["ราคาตัน"].Value.ToString(), 2);
                //data.scoopName = dgvDailyReport.CurrentRow.Cells["ชื่อผู้ตัก"].Value.ToString();
                Weight.StoneType = strNotEmty(dgvDailyReport.CurrentRow.Cells["ชนิดหิน"].Value.ToString());
                Weight.WeightIn = dgvDailyReport.CurrentRow.Cells["น้ำหนักรถ"].Value.ToString();
                Weight.DateIn = dateFormatToPrint(dgvDailyReport.CurrentRow.Cells["วันที่ชั่งเข้า"].Value.ToString());
                Weight.TimeIn = strNotEmty(dgvDailyReport.CurrentRow.Cells["เวลาชั่งเข้า"].Value.ToString());
                Weight.WeightOut = dgvDailyReport.CurrentRow.Cells["น้ำหนักรวม"].Value.ToString();
                Weight.DateOut = dateFormatToPrint(dgvDailyReport.CurrentRow.Cells["วันที่ชั่งออก"].Value.ToString());
                Weight.TimeOut = strNotEmty(dgvDailyReport.CurrentRow.Cells["เวลาชั่งออก"].Value.ToString());
                Weight.WeightTotal = dgvDailyReport.CurrentRow.Cells["น้ำหนักสินค้า"].Value.ToString();
                Weight.Q = dgvDailyReport.CurrentRow.Cells["คิว"].Value.ToString();
                Weight.ApproveName = strNotEmty(dgvDailyReport.CurrentRow.Cells["ชื่อผู้อนุมัติจ่าย"].Value.ToString());
                Weight.AmountVat = numberFormat(dgvDailyReport.CurrentRow.Cells["จำนวนเงินสุทธิ"].Value.ToString(), 2);
                Weight.VatType = dgvDailyReport.CurrentRow.Cells["ชนิดvat"].Value.ToString();
                Weight.StoneColor = strNotEmty(dgvDailyReport.CurrentRow.Cells["ประเภทหิน"].Value.ToString());
                Weight.Site = " ";
                Weight.Team = strNotEmty(dgvDailyReport.CurrentRow.Cells["ทีม"].Value.ToString());
                Weight.Clean = dgvDailyReport.CurrentRow.Cells["ล้าง"].Value.ToString();
                Weight.Vat = numberFormat(dgvDailyReport.CurrentRow.Cells["vat"].Value.ToString(), 2);
                Weight.Transport = strNotEmty(dgvDailyReport.CurrentRow.Cells["ขนส่ง"].Value.ToString());
                Weight.ScoopName = strNotEmty(dgvDailyReport.CurrentRow.Cells["ชื่อผู้ตัก"].Value.ToString());

                Company.TTelephone = "โทร";
                Company.TEmail = "E-mail";

                Company.TDocName = " ";

                FPrint f = new FPrint();
                f.ShowDialog();

            }
            else
            {
                MessageBox.Show("ไม่พบข้อมูลที่ต้องการปริ้นบิล", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void label32_Click(object sender, EventArgs e)
        {

        }

        private void btPrintSummary_Click(object sender, EventArgs e)
        {
            //sql
            dl.connect();
            StringBuilder sql = new StringBuilder();
            sql.Append("select * from weight where วันที่ BETWEEN '" + dtFromSummary.Value.ToString("yyyy-MM-dd") + "' AND '" + dtToSummary.Value.ToString("yyyy-MM-dd") + "' ");
            if (cbbCustomerSummary.SelectedIndex != 0)
                sql.Append(" AND ลูกค้า = '" + cbbCustomerSummary.Text + "' ");
            if (cbbDriverSummary.SelectedIndex != 0)
                sql.Append(" AND คนขับ = '" + cbbDriverSummary.Text + "' ");
            if (cbbStone.SelectedIndex != 0)
                sql.Append(" AND ชนิดหิน = '" + cbbStone.Text + "' ");
            sql.Append(" ORDER BY วันที่ ");

            OdbcDataAdapter cmd = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            dl.close();

            //set parameter
            WeightTempReport.DateFrom = dtFromSummary.Text;
            WeightTempReport.DateTo = dtToSummary.Text;

            //open winform
            Microsoft.Reporting.WinForms.ReportDataSource rds = new Microsoft.Reporting.WinForms.ReportDataSource("summaryDataSet", dt);
            FPrintSummary fp = new FPrintSummary(rds);
            fp.ShowDialog();

        }
    }  
}
