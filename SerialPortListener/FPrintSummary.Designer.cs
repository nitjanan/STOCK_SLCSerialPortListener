namespace SerialPortListener
{
    partial class FPrintSummary
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rvSummaryReport = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rvSummaryReport
            // 
            this.rvSummaryReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rvSummaryReport.LocalReport.ReportEmbeddedResource = "SerialPortListener.SummaryReport.rdlc";
            this.rvSummaryReport.Location = new System.Drawing.Point(0, 0);
            this.rvSummaryReport.Name = "rvSummaryReport";
            this.rvSummaryReport.ServerReport.BearerToken = null;
            this.rvSummaryReport.Size = new System.Drawing.Size(800, 450);
            this.rvSummaryReport.TabIndex = 0;
            // 
            // FPrintSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.rvSummaryReport);
            this.Name = "FPrintSummary";
            this.Text = "รายงานการชั่งแบบสรุป";
            this.Load += new System.EventHandler(this.FPrintSummary_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rvSummaryReport;
    }
}