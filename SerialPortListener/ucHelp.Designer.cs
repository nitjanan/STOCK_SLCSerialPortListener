namespace SerialPortListener
{
    partial class ucHelp
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gbConnection = new System.Windows.Forms.GroupBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.cboPort = new System.Windows.Forms.ComboBox();
            this.lblBaud = new System.Windows.Forms.Label();
            this.cboBaud = new System.Windows.Forms.ComboBox();
            this.lblParity = new System.Windows.Forms.Label();
            this.cboParity = new System.Windows.Forms.ComboBox();
            this.lblDataBits = new System.Windows.Forms.Label();
            this.cboDataBits = new System.Windows.Forms.ComboBox();
            this.lblStopBits = new System.Windows.Forms.Label();
            this.cboStopBits = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSavePort = new System.Windows.Forms.Button();
            this.gbWeightFormat = new System.Windows.Forms.GroupBox();
            this.lblWeightFormat = new System.Windows.Forms.Label();
            this.cboWeightFormat = new System.Windows.Forms.ComboBox();
            this.lblTestData = new System.Windows.Forms.Label();
            this.tbTestRawData = new System.Windows.Forms.TextBox();
            this.btnTestParse = new System.Windows.Forms.Button();
            this.lblTestResult = new System.Windows.Forms.Label();
            this.tbTestParsedWeight = new System.Windows.Forms.TextBox();
            this.gbMonitor = new System.Windows.Forms.GroupBox();
            this.txtDataReceived = new System.Windows.Forms.TextBox();
            this.timerRx = new System.Windows.Forms.Timer(this.components);
            this.gbConnection.SuspendLayout();
            this.gbWeightFormat.SuspendLayout();
            this.gbMonitor.SuspendLayout();
            this.SuspendLayout();
            //
            // gbConnection
            //
            this.gbConnection.Controls.Add(this.lblPort);
            this.gbConnection.Controls.Add(this.cboPort);
            this.gbConnection.Controls.Add(this.lblBaud);
            this.gbConnection.Controls.Add(this.cboBaud);
            this.gbConnection.Controls.Add(this.lblParity);
            this.gbConnection.Controls.Add(this.cboParity);
            this.gbConnection.Controls.Add(this.lblDataBits);
            this.gbConnection.Controls.Add(this.cboDataBits);
            this.gbConnection.Controls.Add(this.lblStopBits);
            this.gbConnection.Controls.Add(this.cboStopBits);
            this.gbConnection.Controls.Add(this.btnStart);
            this.gbConnection.Controls.Add(this.btnStop);
            this.gbConnection.Controls.Add(this.btnSavePort);
            this.gbConnection.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbConnection.Location = new System.Drawing.Point(20, 15);
            this.gbConnection.Name = "gbConnection";
            this.gbConnection.Padding = new System.Windows.Forms.Padding(15, 10, 15, 15);
            this.gbConnection.Size = new System.Drawing.Size(340, 320);
            this.gbConnection.TabIndex = 0;
            this.gbConnection.TabStop = false;
            this.gbConnection.Text = "การเชื่อมต่อพอร์ต";
            //
            // lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPort.Location = new System.Drawing.Point(15, 38);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(78, 20);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "พอร์ต (Port)";
            //
            // cboPort
            //
            this.cboPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPort.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Location = new System.Drawing.Point(130, 35);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(190, 28);
            this.cboPort.TabIndex = 1;
            //
            // lblBaud
            //
            this.lblBaud.AutoSize = true;
            this.lblBaud.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBaud.Location = new System.Drawing.Point(15, 78);
            this.lblBaud.Name = "lblBaud";
            this.lblBaud.Size = new System.Drawing.Size(97, 20);
            this.lblBaud.TabIndex = 2;
            this.lblBaud.Text = "อัตราบอด (Baud)";
            //
            // cboBaud
            //
            this.cboBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBaud.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboBaud.FormattingEnabled = true;
            this.cboBaud.Location = new System.Drawing.Point(130, 75);
            this.cboBaud.Name = "cboBaud";
            this.cboBaud.Size = new System.Drawing.Size(190, 28);
            this.cboBaud.TabIndex = 3;
            //
            // lblParity
            //
            this.lblParity.AutoSize = true;
            this.lblParity.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParity.Location = new System.Drawing.Point(15, 118);
            this.lblParity.Name = "lblParity";
            this.lblParity.Size = new System.Drawing.Size(103, 20);
            this.lblParity.TabIndex = 4;
            this.lblParity.Text = "พาริตี้ (Parity)";
            //
            // cboParity
            //
            this.cboParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParity.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboParity.FormattingEnabled = true;
            this.cboParity.Location = new System.Drawing.Point(130, 115);
            this.cboParity.Name = "cboParity";
            this.cboParity.Size = new System.Drawing.Size(190, 28);
            this.cboParity.TabIndex = 5;
            //
            // lblDataBits
            //
            this.lblDataBits.AutoSize = true;
            this.lblDataBits.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDataBits.Location = new System.Drawing.Point(15, 158);
            this.lblDataBits.Name = "lblDataBits";
            this.lblDataBits.Size = new System.Drawing.Size(115, 20);
            this.lblDataBits.TabIndex = 6;
            this.lblDataBits.Text = "ดาต้าบิต (DataBits)";
            //
            // cboDataBits
            //
            this.cboDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDataBits.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDataBits.FormattingEnabled = true;
            this.cboDataBits.Location = new System.Drawing.Point(130, 155);
            this.cboDataBits.Name = "cboDataBits";
            this.cboDataBits.Size = new System.Drawing.Size(190, 28);
            this.cboDataBits.TabIndex = 7;
            //
            // lblStopBits
            //
            this.lblStopBits.AutoSize = true;
            this.lblStopBits.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStopBits.Location = new System.Drawing.Point(15, 198);
            this.lblStopBits.Name = "lblStopBits";
            this.lblStopBits.Size = new System.Drawing.Size(113, 20);
            this.lblStopBits.TabIndex = 8;
            this.lblStopBits.Text = "สต็อปบิต (StopBits)";
            //
            // cboStopBits
            //
            this.cboStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStopBits.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboStopBits.FormattingEnabled = true;
            this.cboStopBits.Location = new System.Drawing.Point(130, 195);
            this.cboStopBits.Name = "cboStopBits";
            this.cboStopBits.Size = new System.Drawing.Size(190, 28);
            this.cboStopBits.TabIndex = 9;
            //
            // btnStart
            //
            this.btnStart.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(15, 240);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 34);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "เริ่มอ่าน (Start)";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnStop
            //
            this.btnStop.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(170, 240);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(150, 34);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "หยุดอ่าน (Stop)";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // btnSavePort
            //
            this.btnSavePort.BackColor = System.Drawing.Color.White;
            this.btnSavePort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePort.Font = new System.Drawing.Font("Century Gothic", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePort.ForeColor = System.Drawing.Color.Green;
            this.btnSavePort.Location = new System.Drawing.Point(15, 282);
            this.btnSavePort.Name = "btnSavePort";
            this.btnSavePort.Size = new System.Drawing.Size(305, 32);
            this.btnSavePort.TabIndex = 12;
            this.btnSavePort.Text = "บันทึกการตั้งค่า";
            this.btnSavePort.UseVisualStyleBackColor = false;
            this.btnSavePort.Click += new System.EventHandler(this.btnSavePort_Click);
            //
            // gbWeightFormat
            //
            this.gbWeightFormat.Controls.Add(this.lblWeightFormat);
            this.gbWeightFormat.Controls.Add(this.cboWeightFormat);
            this.gbWeightFormat.Controls.Add(this.lblTestData);
            this.gbWeightFormat.Controls.Add(this.tbTestRawData);
            this.gbWeightFormat.Controls.Add(this.btnTestParse);
            this.gbWeightFormat.Controls.Add(this.lblTestResult);
            this.gbWeightFormat.Controls.Add(this.tbTestParsedWeight);
            this.gbWeightFormat.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbWeightFormat.Location = new System.Drawing.Point(20, 345);
            this.gbWeightFormat.Name = "gbWeightFormat";
            this.gbWeightFormat.Padding = new System.Windows.Forms.Padding(15, 10, 15, 15);
            this.gbWeightFormat.Size = new System.Drawing.Size(340, 235);
            this.gbWeightFormat.TabIndex = 1;
            this.gbWeightFormat.TabStop = false;
            this.gbWeightFormat.Text = "รูปแบบข้อมูลตาชั่ง";
            //
            // lblWeightFormat
            //
            this.lblWeightFormat.AutoSize = true;
            this.lblWeightFormat.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWeightFormat.Location = new System.Drawing.Point(15, 35);
            this.lblWeightFormat.Name = "lblWeightFormat";
            this.lblWeightFormat.Size = new System.Drawing.Size(101, 20);
            this.lblWeightFormat.TabIndex = 0;
            this.lblWeightFormat.Text = "เลือกรูปแบบ";
            //
            // cboWeightFormat
            //
            this.cboWeightFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWeightFormat.Font = new System.Drawing.Font("Century Gothic", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboWeightFormat.FormattingEnabled = true;
            this.cboWeightFormat.Location = new System.Drawing.Point(15, 58);
            this.cboWeightFormat.Name = "cboWeightFormat";
            this.cboWeightFormat.Size = new System.Drawing.Size(305, 27);
            this.cboWeightFormat.TabIndex = 1;
            this.cboWeightFormat.SelectedIndexChanged += new System.EventHandler(this.cboWeightFormat_SelectedIndexChanged);
            //
            // lblTestData
            //
            this.lblTestData.AutoSize = true;
            this.lblTestData.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestData.Location = new System.Drawing.Point(15, 98);
            this.lblTestData.Name = "lblTestData";
            this.lblTestData.Size = new System.Drawing.Size(146, 20);
            this.lblTestData.TabIndex = 2;
            this.lblTestData.Text = "ทดสอบข้อมูลดิบ";
            //
            // tbTestRawData
            //
            this.tbTestRawData.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTestRawData.Location = new System.Drawing.Point(15, 121);
            this.tbTestRawData.Name = "tbTestRawData";
            this.tbTestRawData.Size = new System.Drawing.Size(195, 25);
            this.tbTestRawData.TabIndex = 3;
            //
            // btnTestParse
            //
            this.btnTestParse.Font = new System.Drawing.Font("Century Gothic", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestParse.Location = new System.Drawing.Point(220, 119);
            this.btnTestParse.Name = "btnTestParse";
            this.btnTestParse.Size = new System.Drawing.Size(100, 29);
            this.btnTestParse.TabIndex = 4;
            this.btnTestParse.Text = "ทดสอบ";
            this.btnTestParse.UseVisualStyleBackColor = true;
            this.btnTestParse.Click += new System.EventHandler(this.btnTestParse_Click);
            //
            // lblTestResult
            //
            this.lblTestResult.AutoSize = true;
            this.lblTestResult.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestResult.Location = new System.Drawing.Point(15, 160);
            this.lblTestResult.Name = "lblTestResult";
            this.lblTestResult.Size = new System.Drawing.Size(68, 20);
            this.lblTestResult.TabIndex = 5;
            this.lblTestResult.Text = "ผลลัพธ์";
            //
            // tbTestParsedWeight
            //
            this.tbTestParsedWeight.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbTestParsedWeight.Location = new System.Drawing.Point(15, 183);
            this.tbTestParsedWeight.Name = "tbTestParsedWeight";
            this.tbTestParsedWeight.ReadOnly = true;
            this.tbTestParsedWeight.Size = new System.Drawing.Size(305, 29);
            this.tbTestParsedWeight.TabIndex = 6;
            this.tbTestParsedWeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // gbMonitor
            //
            this.gbMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMonitor.Controls.Add(this.txtDataReceived);
            this.gbMonitor.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbMonitor.Location = new System.Drawing.Point(375, 15);
            this.gbMonitor.Name = "gbMonitor";
            this.gbMonitor.Padding = new System.Windows.Forms.Padding(15, 10, 15, 15);
            this.gbMonitor.Size = new System.Drawing.Size(405, 565);
            this.gbMonitor.TabIndex = 2;
            this.gbMonitor.TabStop = false;
            this.gbMonitor.Text = "ข้อมูลดิบที่ได้รับ";
            //
            // txtDataReceived
            //
            this.txtDataReceived.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataReceived.BackColor = System.Drawing.Color.White;
            this.txtDataReceived.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataReceived.Location = new System.Drawing.Point(15, 33);
            this.txtDataReceived.Multiline = true;
            this.txtDataReceived.Name = "txtDataReceived";
            this.txtDataReceived.ReadOnly = true;
            this.txtDataReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDataReceived.Size = new System.Drawing.Size(375, 517);
            this.txtDataReceived.TabIndex = 0;
            //
            // timerRx
            //
            this.timerRx.Interval = 200;
            this.timerRx.Tick += new System.EventHandler(this.timerRx_Tick);
            //
            // ucHelp
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbMonitor);
            this.Controls.Add(this.gbWeightFormat);
            this.Controls.Add(this.gbConnection);
            this.Name = "ucHelp";
            this.Size = new System.Drawing.Size(800, 600);
            this.Load += new System.EventHandler(this.ucHelp_Load);
            this.gbConnection.ResumeLayout(false);
            this.gbConnection.PerformLayout();
            this.gbWeightFormat.ResumeLayout(false);
            this.gbWeightFormat.PerformLayout();
            this.gbMonitor.ResumeLayout(false);
            this.gbMonitor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbConnection;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.Label lblBaud;
        private System.Windows.Forms.ComboBox cboBaud;
        private System.Windows.Forms.Label lblParity;
        private System.Windows.Forms.ComboBox cboParity;
        private System.Windows.Forms.Label lblDataBits;
        private System.Windows.Forms.ComboBox cboDataBits;
        private System.Windows.Forms.Label lblStopBits;
        private System.Windows.Forms.ComboBox cboStopBits;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSavePort;
        private System.Windows.Forms.GroupBox gbWeightFormat;
        private System.Windows.Forms.Label lblWeightFormat;
        private System.Windows.Forms.ComboBox cboWeightFormat;
        private System.Windows.Forms.Label lblTestData;
        private System.Windows.Forms.TextBox tbTestRawData;
        private System.Windows.Forms.Button btnTestParse;
        private System.Windows.Forms.Label lblTestResult;
        private System.Windows.Forms.TextBox tbTestParsedWeight;
        private System.Windows.Forms.GroupBox gbMonitor;
        private System.Windows.Forms.TextBox txtDataReceived;
        private System.Windows.Forms.Timer timerRx;
    }
}
