using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialPortListener
{
    public partial class FPrintSummary : Form
    {
        Microsoft.Reporting.WinForms.ReportDataSource _rs = new Microsoft.Reporting.WinForms.ReportDataSource();
        public FPrintSummary(Microsoft.Reporting.WinForms.ReportDataSource rs)
        {
            InitializeComponent();
            _rs = rs;
        }

        private void FPrintSummary_Load(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportParameter[] p = new Microsoft.Reporting.WinForms.ReportParameter[] {
                    new Microsoft.Reporting.WinForms.ReportParameter("PDateFrom",WeightTempReport.DateFrom),
                    new Microsoft.Reporting.WinForms.ReportParameter("PDateTo",WeightTempReport.DateTo),
                };

                this.rvSummaryReport.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                this.rvSummaryReport.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                this.rvSummaryReport.LocalReport.DataSources.Clear();
                this.rvSummaryReport.LocalReport.DataSources.Add(_rs);
                this.rvSummaryReport.LocalReport.SetParameters(p);
                this.rvSummaryReport.LocalReport.DisplayName = "รายงานการชั่งแบบสรุปประจำวันที่ " + WeightTempReport.DateFrom.Replace('/', '-') + " ถึง " + WeightTempReport.DateTo.Replace('/', '-');
                this.rvSummaryReport.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ReportErrorHelper.BuildMessage(ex), "พิมพ์รายงานไม่สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
    }
}
