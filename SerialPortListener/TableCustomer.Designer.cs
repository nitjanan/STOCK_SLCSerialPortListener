namespace SerialPortListener
{
    partial class TableCustomer
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
            this.components = new System.ComponentModel.Container();
            this.btSave = new System.Windows.Forms.Button();
            this.dgvCustomer = new System.Windows.Forms.DataGridView();
            this.basecustomerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.customerDataSet = new SerialPortListener.customerDataSet();
            this.base_customerTableAdapter = new SerialPortListener.customerDataSetTableAdapters.base_customerTableAdapter();
            this.รหัสลูกค้า = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ชื่อลูกค้า = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ที่อยู่ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ส่งที่ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basecustomerBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customerDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // btSave
            // 
            this.btSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btSave.BackColor = System.Drawing.Color.Thistle;
            this.btSave.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSave.Location = new System.Drawing.Point(683, 484);
            this.btSave.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(96, 34);
            this.btSave.TabIndex = 1;
            this.btSave.Text = "บันทึก";
            this.btSave.UseVisualStyleBackColor = false;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // dgvCustomer
            // 
            this.dgvCustomer.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.dgvCustomer.AutoGenerateColumns = false;
            this.dgvCustomer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCustomer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.รหัสลูกค้า,
            this.ชื่อลูกค้า,
            this.ที่อยู่,
            this.ส่งที่});
            this.dgvCustomer.DataSource = this.basecustomerBindingSource;
            this.dgvCustomer.Location = new System.Drawing.Point(16, 27);
            this.dgvCustomer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvCustomer.Name = "dgvCustomer";
            this.dgvCustomer.ShowRowErrors = false;
            this.dgvCustomer.Size = new System.Drawing.Size(763, 448);
            this.dgvCustomer.TabIndex = 2;
            this.dgvCustomer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvCustomer_KeyDown);
            // 
            // basecustomerBindingSource
            // 
            this.basecustomerBindingSource.DataMember = "base_customer";
            this.basecustomerBindingSource.DataSource = this.customerDataSet;
            // 
            // customerDataSet
            // 
            this.customerDataSet.DataSetName = "customerDataSet";
            this.customerDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // base_customerTableAdapter
            // 
            this.base_customerTableAdapter.ClearBeforeFill = true;
            // 
            // รหัสลูกค้า
            // 
            this.รหัสลูกค้า.DataPropertyName = "รหัสลูกค้า";
            this.รหัสลูกค้า.HeaderText = "รหัสหน้างาน";
            this.รหัสลูกค้า.Name = "รหัสลูกค้า";
            this.รหัสลูกค้า.Width = 150;
            // 
            // ชื่อลูกค้า
            // 
            this.ชื่อลูกค้า.DataPropertyName = "ชื่อลูกค้า";
            this.ชื่อลูกค้า.HeaderText = "ชื่อหน้างาน";
            this.ชื่อลูกค้า.Name = "ชื่อลูกค้า";
            this.ชื่อลูกค้า.Width = 300;
            // 
            // ที่อยู่
            // 
            this.ที่อยู่.DataPropertyName = "ที่อยู่";
            this.ที่อยู่.HeaderText = "ที่อยู่";
            this.ที่อยู่.Name = "ที่อยู่";
            this.ที่อยู่.Width = 260;
            // 
            // ส่งที่
            // 
            this.ส่งที่.DataPropertyName = "ส่งที่";
            this.ส่งที่.HeaderText = "ส่งที่";
            this.ส่งที่.Name = "ส่งที่";
            this.ส่งที่.Visible = false;
            this.ส่งที่.Width = 150;
            // 
            // TableCustomer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Lavender;
            this.ClientSize = new System.Drawing.Size(798, 532);
            this.Controls.Add(this.dgvCustomer);
            this.Controls.Add(this.btSave);
            this.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "TableCustomer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "หน้างาน";
            this.Load += new System.EventHandler(this.TableCustomer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basecustomerBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customerDataSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btSave;
        private System.Windows.Forms.DataGridView dgvCustomer;
        private customerDataSet customerDataSet;
        private System.Windows.Forms.BindingSource basecustomerBindingSource;
        private customerDataSetTableAdapters.base_customerTableAdapter base_customerTableAdapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn รหัสลูกค้า;
        private System.Windows.Forms.DataGridViewTextBoxColumn ชื่อลูกค้า;
        private System.Windows.Forms.DataGridViewTextBoxColumn ที่อยู่;
        private System.Windows.Forms.DataGridViewTextBoxColumn ส่งที่;
    }
}