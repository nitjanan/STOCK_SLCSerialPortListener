namespace SerialPortListener
{
    partial class ucBackup
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbDLSetting = new System.Windows.Forms.Label();
            this.btDLSetting = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbDLWeight = new System.Windows.Forms.Label();
            this.btDLWeight = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lbULWeightDate = new System.Windows.Forms.Label();
            this.tbdateULWeight = new System.Windows.Forms.DateTimePicker();
            this.btULWeight = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbCheckUpdate = new System.Windows.Forms.Label();
            this.btnCheckUpdate = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.lbDLSetting);
            this.groupBox1.Controls.Add(this.btDLSetting);
            this.groupBox1.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(25, 20);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.groupBox1.Size = new System.Drawing.Size(900, 85);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ดาวน์โหลดการตั้งค่า";
            //
            // lbDLSetting
            //
            this.lbDLSetting.AutoSize = true;
            this.lbDLSetting.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDLSetting.Location = new System.Drawing.Point(20, 40);
            this.lbDLSetting.Name = "lbDLSetting";
            this.lbDLSetting.Size = new System.Drawing.Size(279, 20);
            this.lbDLSetting.TabIndex = 0;
            this.lbDLSetting.Text = "ดาวน์โหลดข้อมูลตั้งค่าล่าสุดจากเซิร์ฟเวอร์";
            //
            // btDLSetting
            //
            this.btDLSetting.Location = new System.Drawing.Point(710, 33);
            this.btDLSetting.Name = "btDLSetting";
            this.btDLSetting.Size = new System.Drawing.Size(150, 35);
            this.btDLSetting.TabIndex = 1;
            this.btDLSetting.Text = "ดาวน์โหลด";
            this.btDLSetting.UseVisualStyleBackColor = true;
            this.btDLSetting.Click += new System.EventHandler(this.btDLSetting_Click);
            //
            // groupBox2
            //
            this.groupBox2.Controls.Add(this.lbDLWeight);
            this.groupBox2.Controls.Add(this.btDLWeight);
            this.groupBox2.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(25, 120);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.groupBox2.Size = new System.Drawing.Size(900, 85);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ดาวน์โหลดรายการชั่งที่แก้ไข";
            //
            // lbDLWeight
            //
            this.lbDLWeight.AutoSize = true;
            this.lbDLWeight.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDLWeight.Location = new System.Drawing.Point(20, 40);
            this.lbDLWeight.Name = "lbDLWeight";
            this.lbDLWeight.Size = new System.Drawing.Size(279, 20);
            this.lbDLWeight.TabIndex = 0;
            this.lbDLWeight.Text = "ดาวน์โหลดรายการชั่งที่มีการแก้ไขจากเซิร์ฟเวอร์";
            //
            // btDLWeight
            //
            this.btDLWeight.Location = new System.Drawing.Point(710, 33);
            this.btDLWeight.Name = "btDLWeight";
            this.btDLWeight.Size = new System.Drawing.Size(150, 35);
            this.btDLWeight.TabIndex = 1;
            this.btDLWeight.Text = "ดาวน์โหลด";
            this.btDLWeight.UseVisualStyleBackColor = true;
            this.btDLWeight.Click += new System.EventHandler(this.btDLWeight_Click);
            //
            // groupBox3
            //
            this.groupBox3.Controls.Add(this.lbULWeightDate);
            this.groupBox3.Controls.Add(this.tbdateULWeight);
            this.groupBox3.Controls.Add(this.btULWeight);
            this.groupBox3.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(25, 220);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.groupBox3.Size = new System.Drawing.Size(900, 90);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "อัปโหลดขึ้น WebApp";
            //
            // lbULWeightDate
            //
            this.lbULWeightDate.AutoSize = true;
            this.lbULWeightDate.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbULWeightDate.Location = new System.Drawing.Point(20, 45);
            this.lbULWeightDate.Name = "lbULWeightDate";
            this.lbULWeightDate.Size = new System.Drawing.Size(140, 20);
            this.lbULWeightDate.TabIndex = 0;
            this.lbULWeightDate.Text = "วันที่ต้องการอัปโหลด :";
            //
            // tbdateULWeight
            //
            this.tbdateULWeight.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbdateULWeight.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.tbdateULWeight.Location = new System.Drawing.Point(180, 41);
            this.tbdateULWeight.Name = "tbdateULWeight";
            this.tbdateULWeight.Size = new System.Drawing.Size(200, 27);
            this.tbdateULWeight.TabIndex = 1;
            //
            // btULWeight
            //
            this.btULWeight.Enabled = false;
            this.btULWeight.Location = new System.Drawing.Point(710, 38);
            this.btULWeight.Name = "btULWeight";
            this.btULWeight.Size = new System.Drawing.Size(150, 35);
            this.btULWeight.TabIndex = 2;
            this.btULWeight.Text = "อัปโหลด";
            this.btULWeight.UseVisualStyleBackColor = true;
            this.btULWeight.Click += new System.EventHandler(this.btULWeight_Click);
            //
            // groupBox4
            //
            this.groupBox4.Controls.Add(this.lbCheckUpdate);
            this.groupBox4.Controls.Add(this.btnCheckUpdate);
            this.groupBox4.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(25, 325);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.groupBox4.Size = new System.Drawing.Size(900, 85);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ตรวจสอบอัพเดทโปรแกรม";
            //
            // lbCheckUpdate
            //
            this.lbCheckUpdate.AutoSize = true;
            this.lbCheckUpdate.Font = new System.Drawing.Font("Century Gothic", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCheckUpdate.Location = new System.Drawing.Point(20, 40);
            this.lbCheckUpdate.Name = "lbCheckUpdate";
            this.lbCheckUpdate.Size = new System.Drawing.Size(279, 20);
            this.lbCheckUpdate.TabIndex = 0;
            this.lbCheckUpdate.Text = "ตรวจสอบและติดตั้งเวอร์ชันล่าสุดของโปรแกรม";
            //
            // btnCheckUpdate
            //
            this.btnCheckUpdate.Location = new System.Drawing.Point(710, 33);
            this.btnCheckUpdate.Name = "btnCheckUpdate";
            this.btnCheckUpdate.Size = new System.Drawing.Size(150, 35);
            this.btnCheckUpdate.TabIndex = 1;
            this.btnCheckUpdate.Text = "ตรวจสอบอัพเดท";
            this.btnCheckUpdate.UseVisualStyleBackColor = true;
            this.btnCheckUpdate.Click += new System.EventHandler(this.btnCheckUpdate_Click);
            //
            // ucBackup
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ucBackup";
            this.Size = new System.Drawing.Size(965, 460);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbDLSetting;
        private System.Windows.Forms.Button btDLSetting;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbDLWeight;
        private System.Windows.Forms.Button btDLWeight;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lbULWeightDate;
        private System.Windows.Forms.Button btULWeight;
        private System.Windows.Forms.DateTimePicker tbdateULWeight;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lbCheckUpdate;
        private System.Windows.Forms.Button btnCheckUpdate;
    }
}
