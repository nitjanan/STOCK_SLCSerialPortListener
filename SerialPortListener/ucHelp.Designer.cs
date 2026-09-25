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
            this.gbWeightFormat = new System.Windows.Forms.GroupBox();
            this.lblWeightFormat = new System.Windows.Forms.Label();
            this.cboWeightFormat = new System.Windows.Forms.ComboBox();
            this.lblWeightPreview = new System.Windows.Forms.Label();
            this.tbWeightPreview = new System.Windows.Forms.TextBox();
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
            this.lblRawData = new System.Windows.Forms.Label();
            this.txtDataReceived = new System.Windows.Forms.TextBox();
            this.btnSavePort = new System.Windows.Forms.Button();
            this.timerRx = new System.Windows.Forms.Timer(this.components);
            this.gbWeightFormat.SuspendLayout();
            this.gbConnection.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbWeightFormat
            // 
            this.gbWeightFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWeightFormat.Controls.Add(this.lblWeightFormat);
            this.gbWeightFormat.Controls.Add(this.cboWeightFormat);
            this.gbWeightFormat.Controls.Add(this.lblWeightPreview);
            this.gbWeightFormat.Controls.Add(this.tbWeightPreview);
            this.gbWeightFormat.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbWeightFormat.Location = new System.Drawing.Point(15, 15);
            this.gbWeightFormat.Name = "gbWeightFormat";
            this.gbWeightFormat.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.gbWeightFormat.Size = new System.Drawing.Size(661, 100);
            this.gbWeightFormat.TabIndex = 0;
            this.gbWeightFormat.TabStop = false;
            this.gbWeightFormat.Text = "รูปแบบตาชั่ง";
            // 
            // lblWeightFormat
            // 
            this.lblWeightFormat.AutoSize = true;
            this.lblWeightFormat.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWeightFormat.Location = new System.Drawing.Point(21, 24);
            this.lblWeightFormat.Name = "lblWeightFormat";
            this.lblWeightFormat.Size = new System.Drawing.Size(80, 20);
            this.lblWeightFormat.TabIndex = 0;
            this.lblWeightFormat.Text = "รูปแบบตาชั่ง";
            // 
            // cboWeightFormat
            // 
            this.cboWeightFormat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboWeightFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboWeightFormat.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboWeightFormat.FormattingEnabled = true;
            this.cboWeightFormat.Location = new System.Drawing.Point(161, 20);
            this.cboWeightFormat.Name = "cboWeightFormat";
            this.cboWeightFormat.Size = new System.Drawing.Size(388, 29);
            this.cboWeightFormat.TabIndex = 1;
            this.cboWeightFormat.SelectedIndexChanged += new System.EventHandler(this.cboWeightFormat_SelectedIndexChanged);
            // 
            // lblWeightPreview
            // 
            this.lblWeightPreview.AutoSize = true;
            this.lblWeightPreview.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWeightPreview.Location = new System.Drawing.Point(21, 62);
            this.lblWeightPreview.Name = "lblWeightPreview";
            this.lblWeightPreview.Size = new System.Drawing.Size(100, 20);
            this.lblWeightPreview.TabIndex = 2;
            this.lblWeightPreview.Text = "น้ำหนักที่อ่านได้";
            // 
            // tbWeightPreview
            // 
            this.tbWeightPreview.BackColor = System.Drawing.Color.Black;
            this.tbWeightPreview.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbWeightPreview.ForeColor = System.Drawing.Color.LightGreen;
            this.tbWeightPreview.Location = new System.Drawing.Point(161, 54);
            this.tbWeightPreview.Name = "tbWeightPreview";
            this.tbWeightPreview.ReadOnly = true;
            this.tbWeightPreview.Size = new System.Drawing.Size(461, 36);
            this.tbWeightPreview.TabIndex = 3;
            this.tbWeightPreview.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbConnection
            // 
            this.gbConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.gbConnection.Controls.Add(this.lblRawData);
            this.gbConnection.Controls.Add(this.txtDataReceived);
            this.gbConnection.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbConnection.Location = new System.Drawing.Point(15, 121);
            this.gbConnection.Name = "gbConnection";
            this.gbConnection.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.gbConnection.Size = new System.Drawing.Size(661, 305);
            this.gbConnection.TabIndex = 1;
            this.gbConnection.TabStop = false;
            this.gbConnection.Text = "ตั้งค่าพอร์ต";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPort.Location = new System.Drawing.Point(20, 40);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(38, 20);
            this.lblPort.TabIndex = 0;
            this.lblPort.Text = "Port";
            // 
            // cboPort
            // 
            this.cboPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPort.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboPort.FormattingEnabled = true;
            this.cboPort.Location = new System.Drawing.Point(140, 37);
            this.cboPort.Name = "cboPort";
            this.cboPort.Size = new System.Drawing.Size(260, 28);
            this.cboPort.TabIndex = 1;
            // 
            // lblBaud
            // 
            this.lblBaud.AutoSize = true;
            this.lblBaud.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBaud.Location = new System.Drawing.Point(20, 82);
            this.lblBaud.Name = "lblBaud";
            this.lblBaud.Size = new System.Drawing.Size(47, 20);
            this.lblBaud.TabIndex = 2;
            this.lblBaud.Text = "Baud";
            // 
            // cboBaud
            // 
            this.cboBaud.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBaud.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboBaud.FormattingEnabled = true;
            this.cboBaud.Location = new System.Drawing.Point(140, 79);
            this.cboBaud.Name = "cboBaud";
            this.cboBaud.Size = new System.Drawing.Size(260, 28);
            this.cboBaud.TabIndex = 3;
            // 
            // lblParity
            // 
            this.lblParity.AutoSize = true;
            this.lblParity.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParity.Location = new System.Drawing.Point(20, 124);
            this.lblParity.Name = "lblParity";
            this.lblParity.Size = new System.Drawing.Size(49, 20);
            this.lblParity.TabIndex = 4;
            this.lblParity.Text = "Parity";
            // 
            // cboParity
            // 
            this.cboParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboParity.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboParity.FormattingEnabled = true;
            this.cboParity.Location = new System.Drawing.Point(140, 121);
            this.cboParity.Name = "cboParity";
            this.cboParity.Size = new System.Drawing.Size(260, 28);
            this.cboParity.TabIndex = 5;
            // 
            // lblDataBits
            // 
            this.lblDataBits.AutoSize = true;
            this.lblDataBits.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDataBits.Location = new System.Drawing.Point(20, 166);
            this.lblDataBits.Name = "lblDataBits";
            this.lblDataBits.Size = new System.Drawing.Size(68, 20);
            this.lblDataBits.TabIndex = 6;
            this.lblDataBits.Text = "DataBits";
            // 
            // cboDataBits
            // 
            this.cboDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDataBits.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboDataBits.FormattingEnabled = true;
            this.cboDataBits.Location = new System.Drawing.Point(140, 163);
            this.cboDataBits.Name = "cboDataBits";
            this.cboDataBits.Size = new System.Drawing.Size(260, 28);
            this.cboDataBits.TabIndex = 7;
            // 
            // lblStopBits
            // 
            this.lblStopBits.AutoSize = true;
            this.lblStopBits.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStopBits.Location = new System.Drawing.Point(20, 208);
            this.lblStopBits.Name = "lblStopBits";
            this.lblStopBits.Size = new System.Drawing.Size(64, 20);
            this.lblStopBits.TabIndex = 8;
            this.lblStopBits.Text = "StopBits";
            // 
            // cboStopBits
            // 
            this.cboStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStopBits.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboStopBits.FormattingEnabled = true;
            this.cboStopBits.Location = new System.Drawing.Point(140, 205);
            this.cboStopBits.Name = "cboStopBits";
            this.cboStopBits.Size = new System.Drawing.Size(260, 28);
            this.cboStopBits.TabIndex = 9;
            // 
            // btnStart
            // 
            this.btnStart.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(24, 248);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(186, 39);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(220, 248);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(180, 39);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblRawData
            // 
            this.lblRawData.AutoSize = true;
            this.lblRawData.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRawData.Location = new System.Drawing.Point(440, 40);
            this.lblRawData.Name = "lblRawData";
            this.lblRawData.Size = new System.Drawing.Size(117, 20);
            this.lblRawData.TabIndex = 12;
            this.lblRawData.Text = "ข้อมูลดิบจากพอร์ต";
            // 
            // txtDataReceived
            // 
            this.txtDataReceived.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataReceived.BackColor = System.Drawing.Color.White;
            this.txtDataReceived.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataReceived.Location = new System.Drawing.Point(440, 65);
            this.txtDataReceived.Multiline = true;
            this.txtDataReceived.Name = "txtDataReceived";
            this.txtDataReceived.ReadOnly = true;
            this.txtDataReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDataReceived.Size = new System.Drawing.Size(201, 225);
            this.txtDataReceived.TabIndex = 13;
            // 
            // btnSavePort
            // 
            this.btnSavePort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePort.BackColor = System.Drawing.Color.SeaGreen;
            this.btnSavePort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePort.Font = new System.Drawing.Font("Century Gothic", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSavePort.ForeColor = System.Drawing.Color.White;
            this.btnSavePort.Location = new System.Drawing.Point(15, 432);
            this.btnSavePort.Name = "btnSavePort";
            this.btnSavePort.Size = new System.Drawing.Size(661, 40);
            this.btnSavePort.TabIndex = 2;
            this.btnSavePort.Text = "บันทึกการตั้งค่า";
            this.btnSavePort.UseVisualStyleBackColor = false;
            this.btnSavePort.Click += new System.EventHandler(this.btnSavePort_Click);
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
            this.Controls.Add(this.btnSavePort);
            this.Controls.Add(this.gbConnection);
            this.Controls.Add(this.gbWeightFormat);
            this.Name = "ucHelp";
            this.Size = new System.Drawing.Size(1000, 665);
            this.Load += new System.EventHandler(this.ucHelp_Load);
            this.gbWeightFormat.ResumeLayout(false);
            this.gbWeightFormat.PerformLayout();
            this.gbConnection.ResumeLayout(false);
            this.gbConnection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbWeightFormat;
        private System.Windows.Forms.Label lblWeightFormat;
        private System.Windows.Forms.ComboBox cboWeightFormat;
        private System.Windows.Forms.Label lblWeightPreview;
        private System.Windows.Forms.TextBox tbWeightPreview;
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
        private System.Windows.Forms.Label lblRawData;
        private System.Windows.Forms.TextBox txtDataReceived;
        private System.Windows.Forms.Button btnSavePort;
        private System.Windows.Forms.Timer timerRx;
    }
}
