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
using System.Diagnostics;

namespace SerialPortListener
{
    public partial class ucSetting : UserControl
    {
        private static ucSetting _instance;
        Datalayer dl = null;
        //scale
        OdbcDataAdapter adtScale;
        DataTable dtScale;
        OdbcCommandBuilder cmbScale;
        //scoop
        OdbcDataAdapter adtScoop;
        DataTable dtScoop;
        OdbcCommandBuilder cmbScoop;
        //stone type
        OdbcDataAdapter adtStoneType;
        DataTable dtStoneType;
        OdbcCommandBuilder cmbStoneType;
        //approve
        OdbcDataAdapter adtApprove;
        DataTable dtApprove;
        OdbcCommandBuilder cmbApprove;
        //customer
        OdbcDataAdapter adtCustomer;
        DataTable dtCustomer;
        OdbcCommandBuilder cmbCustomer;
        //car city
        OdbcDataAdapter adtCarCity;
        DataTable dtCarCity;
        OdbcCommandBuilder cmbCarCity;
        //users
        OdbcDataAdapter adtUsers;
        DataTable dtUsers;
        OdbcCommandBuilder cmbUsers;
        //car team
        OdbcDataAdapter adtCarTeam;
        DataTable dtCarTeam;
        OdbcCommandBuilder cmbCarTeam;
        //car
        OdbcDataAdapter adtCar;
        DataTable dtCar;
        OdbcCommandBuilder cmbCar;

        //driver
        OdbcDataAdapter adtDriver;
        DataTable dtDriver;
        OdbcCommandBuilder cmbDriver;

        //car registration
        OdbcDataAdapter adtCarRegistration;
        DataTable dtCarRegistration;
        OdbcCommandBuilder cmbCarRegistration;

        private static ucSetting Instance
        {
            set 
            {
                if (_instance == null)
                    _instance = new ucSetting();
            }
            get
            {
                if (_instance == null)
                    _instance = new ucSetting();
                return _instance;
            }
        }
        public ucSetting()
        {
            dl = new Datalayer();
            InitializeComponent();
        }

        private void ucSetting_Load(object sender, EventArgs e)
        {
            //this.base_scaleTableAdapter.Fill(this.baseScaleDataSet.base_scale);
            setDataSouceForDGVScale();
            if (!Globals.isPermissionTop() && !Globals.isPermissionAddSetting())
            {
                //แถบรหัสพนักงาน
                tcSetting.TabPages.Remove(tabPage1);
                //แถบuser
                tcSetting.TabPages.Remove(tabPage7);
                //แถบอนุมัติจ่าย
                tcSetting.TabPages.Remove(tabPage4);
                //แถบจังหวัด
                tcSetting.TabPages.Remove(tabPage6);
                //แถบทีม
                tcSetting.TabPages.Remove(tabPage8);
                //แถบรถร่วม
                tcSetting.TabPages.Remove(tabPage9);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tcSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            resetDTGV();
        }

        private void saveAndUpdateDTGV(OdbcCommandBuilder cmb, OdbcDataAdapter adt, DataTable dt, DataGridView dgv, String tableName)
        {
            try
            {
                dl.connect();
                cmb = new OdbcCommandBuilder(adt);
                adt.Update(dt);
                MessageBox.Show("บันทึกข้อมูลเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dl.close();
            }
            catch (System.InvalidOperationException exUpdate)
            {
                updateDTGV(dgv, tableName);
            }
            catch (OdbcException exDuplicate)
            {
                MessageBox.Show("มีรหัสที่ซ้ำกัน กรุณากรอกข้อมูลใหม่", "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("กรุณาลองใหม่อีกครั้ง", "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void resetDTGV() {
            if (tcSetting.SelectedTab == tabPage1)
            {
                //this.base_scaleTableAdapter.Fill(this.baseScaleDataSet.base_scale);
                setDataSouceForDGVScale();
            }
            else if (tcSetting.SelectedTab == tabPage2)
            {
                //this.base_stone_typeTableAdapter.Fill(this.baseStoneTypeDataSet.base_stone_type);
                setDataSouceForDGVStoneType();
            }
            else if (tcSetting.SelectedTab == tabPage3)
            {
                //this.base_scoopTableAdapter.Fill(this.baseScoopDataSet.base_scoop);
                setDataSouceForDGVScoop();
            }
            else if (tcSetting.SelectedTab == tabPage4)
            {
                //this.base_approveTableAdapter.Fill(this.baseApproveDataSet.base_approve);
                setDataSouceForDGVApprove();
            }
            else if (tcSetting.SelectedTab == tabPage5)
            {
                //this.base_customerTableAdapter.Fill(this.baseCustomerDataSet.base_customer);
                setDataSouceForDGVCustomer();
            }
            else if (tcSetting.SelectedTab == tabPage6)
            {
                //this.base_car_cityTableAdapter .Fill(this.baseCarCityDataSet.base_car_city);
                setDataSouceForDGVCarCity();

            }
            else if (tcSetting.SelectedTab == tabPage7)
            {
                //this.usersTableAdapter.Fill(this.usersDataSet.users);
                setDataSouceForDGVUsers();
            }
            else if (tcSetting.SelectedTab == tabPage8)
            {
                //this.base_car_teamTableAdapter.Fill(this.baseCarTeamDataSet.base_car_team);
                setDataSouceForDGVCarTeam();
            }
            else if (tcSetting.SelectedTab == tabPage9)
            {
                fillCarTeamCombo();
                //this.base_carTableAdapter.Fill(this.baseCarDataSet.base_car);
                setDataSouceForDGVCar();
            }
            else if (tcSetting.SelectedTab == tabPage10)
            {
                //this.base_carTableAdapter.Fill(this.baseCarDataSet.base_car);
                setDataSouceForDGVDriver();
            }
            else if (tcSetting.SelectedTab == tabPage11)
            {
                //this.base_carTableAdapter.Fill(this.baseCarDataSet.base_car);
                setDataSouceForDGVCarRegistration();
            }
        }


        private void deleteDTGVOld(DataGridView dgv)
        {
            try
            {
                int rowIndex = dgv.CurrentCell.RowIndex;
                dgv.Rows.RemoveAt(rowIndex);
            }
            catch (Exception ex)
            {
            }
        }


        private void deleteDTGV(DataGridView dgv, String cellName, String tableName)
        {
            int rowIndex = dgv.CurrentCell.RowIndex;
            string id = dgv.CurrentRow.Cells[cellName].Value.ToString();

            //sql delete
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "DELETE FROM public."+ tableName + " where "+ cellName + " = '" + id + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                dgv.Rows.RemoveAt(rowIndex);
                MessageBox.Show("ลบข้อมูลเรียบร้อย", "ลบข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("กรุณาลองใหม่อีกครั้ง", "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dl.close();

            //reset ก่อน save ใหม่
            resetDTGV();
        }

        private void updateDTGV(DataGridView dgv, String tableName)
        {
            //dgv.CurrentRow.ErrorText = "";

            int numCol = dgv.Columns.Count;

            string idName = dgv.Columns[0].Name;
            string idValue = dgv.CurrentRow.Cells[idName].Value.ToString();

            string colOneName = dgv.Columns[1].Name;
            string colOneValue = dgv.CurrentRow.Cells[colOneName].Value.ToString();

            string colTwoName = null;
            string colTwoValue = null;

            if (numCol > 2)
            {
                colTwoName = dgv.Columns[2].Name;
                colTwoValue = dgv.CurrentRow.Cells[colTwoName].Value.ToString();
            }

            //sql update
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            StringBuilder sqlTxt = new StringBuilder();
            sqlTxt.Append("UPDATE public." + tableName + " SET " + colOneName + " = '" + colOneValue + "' ");
            if (numCol > 2)
                sqlTxt.Append(" , " + colTwoName + " = '" + colTwoValue + "' ");
            sqlTxt.Append(" WHERE " + idName + " = '" + idValue + "' ");
            pgCommand.CommandText = sqlTxt.ToString();
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                MessageBox.Show("อัพเดทข้อมูลเรียบร้อย", "ลบข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception)
            {
                MessageBox.Show("กรุณาลองใหม่อีกครั้ง", "ผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dl.close();
        }

        private void fillCarTeamCombo()
        {
            //ล้างก่อน
            cbbCarTeamName.Items.Clear();
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
                    cbbCarTeamName.Items.Add(des);
                }
            }
            catch (Exception)
            {

            }
            dl.close();
            cbbCarTeamName.SelectedIndex = 0;
        }

        /*Base Scale*/
        private void setDataSouceForDGVScale()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_scale ");
                adtScale = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtScale = new DataTable();
                adtScale.Fill(dtScale);
                dgvScale.DataSource = dtScale;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void btSaveScale_Click(object sender, EventArgs e)
        {
            //saveActionScale();
            saveAndUpdateDTGV(cmbScale, adtScale, dtScale, dgvScale , "base_scale");
        }

        private void saveActionScale()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                basescaleBindingSource.EndEdit();
                base_scaleTableAdapter.Update(this.baseScaleDataSet.base_scale);
                MessageBox.Show("บันทึกข้อมูลเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }


        private void dgvScale_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    basescaleBindingSource.RemoveCurrent();
                    saveActionScale();
                }
            }
            */
        }

        private void btDelScale_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //basescaleBindingSource.RemoveCurrent();
                //saveActionScale();
                deleteDTGV(dgvScale, "รหัสพนักงาน", "base_scale");
            }
        }

        /* Base Stone Type*/
        private void setDataSouceForDGVStoneType()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_stone_type ");
                adtStoneType = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtStoneType = new DataTable();
                adtStoneType.Fill(dtStoneType);
                dgvStoneType.DataSource = dtStoneType;
            }
            catch (Exception)
            {
            }
            dl.close();
        } 
        private void btSaveStoneType_Click(object sender, EventArgs e)
        {
            //saveActionStoneType();
            saveAndUpdateDTGV(cmbStoneType, adtStoneType, dtStoneType , dgvStoneType, "base_stone_type");
        }
        private void saveActionStoneType()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                basestonetypeBindingSource.EndEdit();
                base_stone_typeTableAdapter.Update(this.baseStoneTypeDataSet.base_stone_type);
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void dgvStoneType_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    basestonetypeBindingSource.RemoveCurrent();
                    saveActionStoneType();
                }
            }
            */
        }

        private void btDelStoneType_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //basestonetypeBindingSource.RemoveCurrent();
                //saveActionStoneType();
                deleteDTGV(dgvStoneType, "รหัสหิน", "base_stone_type");
            }
        }

        /*Base Scoop*/
        private void setDataSouceForDGVScoop()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_scoop ");
                adtScoop = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtScoop = new DataTable();
                adtScoop.Fill(dtScoop);
                dgvScoop.DataSource = dtScoop;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void btSaveScoop_Click(object sender, EventArgs e)
        {
            //saveActionScoop();
            saveAndUpdateDTGV(cmbScoop, adtScoop, dtScoop, dgvScoop, "base_scoop");
        }
        private void saveActionScoop()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                basescoopBindingSource.EndEdit();
                base_scoopTableAdapter.Update(this.baseScoopDataSet.base_scoop);
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void dgvScoop_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    basescoopBindingSource.RemoveCurrent();
                    saveActionScoop();
                }
            }
            */
        }

        private void btDelScoop_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //basescoopBindingSource.RemoveCurrent();
                //saveActionScoop();
                deleteDTGV(dgvScoop, "รหัสผู้ตัก", "base_scoop");
            }
        }

        /*Base Approve*/
        private void setDataSouceForDGVApprove()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_approve ");
                adtApprove = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtApprove = new DataTable();
                adtApprove.Fill(dtApprove);
                dgvApprove.DataSource = dtApprove;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void btSaveApprove_Click(object sender, EventArgs e)
        {
            //saveActionApprove();
            saveAndUpdateDTGV(cmbApprove, adtApprove, dtApprove, dgvApprove, "base_approve");
        }        
        private void saveActionApprove()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                baseapproveBindingSource.EndEdit();
                base_approveTableAdapter.Update(this.baseApproveDataSet.base_approve);
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void dgvApprove_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    baseapproveBindingSource.RemoveCurrent();
                    saveActionApprove();
                }
            }
            */
        }

        private void btDelApprove_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //baseapproveBindingSource.RemoveCurrent();
                //saveActionApprove();

                deleteDTGV(dgvApprove, "รหัสผู้อนุมัติจ่าย", "base_approve");
            }
        }

        /*Base Customer*/
        private void setDataSouceForDGVCustomer()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_customer ");
                adtCustomer = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtCustomer = new DataTable();
                adtCustomer.Fill(dtCustomer);
                dgvCustomer.DataSource = dtCustomer;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void saveActionCustomer()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                basecustomerBindingSource.EndEdit();
                base_customerTableAdapter.Update(this.baseCustomerDataSet.base_customer);
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btSaveCustomer_Click(object sender, EventArgs e)
        {
            //saveActionCustomer();
            saveAndUpdateDTGV(cmbCustomer, adtCustomer, dtCustomer, dgvCustomer, "base_customer");
        }

        private void dgvCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    basecustomerBindingSource.RemoveCurrent();
                    saveActionCustomer();
                }
            }
            */
        }
        private void btDelCustomer_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //basecustomerBindingSource.RemoveCurrent();
                //saveActionCustomer();

                deleteDTGV(dgvCustomer, "รหัสลูกค้า", "base_customer");
            }
        }

        /*Base Car City*/
        private void setDataSouceForDGVCarCity()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_car_city ");
                adtCarCity = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtCarCity = new DataTable();
                adtCarCity.Fill(dtCarCity);
                dgvCarCity.DataSource = dtCarCity;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void btSaveCity_Click(object sender, EventArgs e)
        {
            //saveActionCarCity();
            saveAndUpdateDTGV(cmbCarCity, adtCarCity, dtCarCity, dgvCarCity, "base_car_city");
        }
        private void saveActionCarCity()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                basecarcityBindingSource.EndEdit();
                base_car_cityTableAdapter.Update(this.baseCarCityDataSet.base_car_city);
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void dgvCarCity_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    basecarcityBindingSource.RemoveCurrent();
                    saveActionCarCity();
                }
            }
            */
        }

        private void btDelCity_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //basecarcityBindingSource.RemoveCurrent();
                //saveActionCarCity();
                deleteDTGV(dgvCarCity, "รหัสจังหวัด", "base_car_city");
            }
        }

        /*Users*/
        private void setDataSouceForDGVUsers()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.users ");
                adtUsers = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtUsers = new DataTable();
                adtUsers.Fill(dtUsers);
                dgvUsers.DataSource = dtUsers;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void saveActionUsers()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                usersBindingSource.EndEdit();
                usersTableAdapter.Update(this.usersDataSet.users);
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btSaveUsers_Click(object sender, EventArgs e)
        {
            //saveActionUsers();
            saveAndUpdateDTGV(cmbUsers, adtUsers, dtUsers, dgvUsers, "users");
        }

        private void dgvUsers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //usersBindingSource.RemoveCurrent();
                    //saveActionUsers();
                    deleteDTGV(dgvUsers, "users_id", "users");
                }
            }
        }

        private void tbText_Leave(object sender, EventArgs e)
        {
            tbEncryption.Text = Utils.hashPassword(tbText.Text);
        }

        /*Base Car Team*/
        private void setDataSouceForDGVCarTeam()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_car_team ");
                adtCarTeam = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtCarTeam = new DataTable();
                adtCarTeam.Fill(dtCarTeam);
                dgvTeamCar.DataSource = dtCarTeam;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        /*Base Car Team*/
        private void setDataSouceForDGVDriver()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_driver ");
                adtDriver = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtDriver = new DataTable();
                adtDriver.Fill(dtDriver);
                dgvDriver.DataSource = dtDriver;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        /*Base Car Registration*/
        private void setDataSouceForDGVCarRegistration()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_car_registration ");
                adtCarRegistration = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtCarRegistration = new DataTable();
                adtCarRegistration.Fill(dtCarRegistration);
                dgvCarRegistration.DataSource = dtCarRegistration;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void btSaveCarTeam_Click(object sender, EventArgs e)
        {
            //saveActionCarTeam();
            saveAndUpdateDTGV(cmbCarTeam, adtCarTeam, dtCarTeam, dgvTeamCar, "base_car_team");
        }
        private void saveActionCarTeam()
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                basecarteamBindingSource.EndEdit();
                base_car_teamTableAdapter.Update(this.baseCarTeamDataSet.base_car_team);
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btDelCarTeam_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //basecarteamBindingSource.RemoveCurrent();
                //saveActionCarTeam();

                deleteDTGV(dgvTeamCar, "รหัสทีม", "base_car_team");
            }
        }

        private void dgvCar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            /*set ค่าที่มาจาก Table base_car_team*/
            if(cbbCarTeamName.SelectedIndex != -1)
                setDataCarTeam();
        }

        private void setDataCarTeam()
        {
            if (dgvCar.Rows.Count > 1)
            {
                    tbCarId.Text = dgvCar.CurrentRow.Cells["รหัสรถร่วม"].Value.ToString();
                    tbCarName.Text = dgvCar.CurrentRow.Cells["ชื่อรถร่วม"].Value.ToString();

                    //เปิดปิดช่องเมื่อมีไม่มีค่า
                    if (tbCarId.Text != "")
                        tbCarId.ReadOnly = true;
                    else
                        tbCarId.ReadOnly = false;
            }
        }

        /*Base Car*/
        private void setDataSouceForDGVCar()
        {
            try
            {
                dl.connect();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT *  FROM public.base_car where รหัสทีม = '" + tbCarTeamId.Text + "'");
                adtCar = new OdbcDataAdapter(sql.ToString(), dl.sqlConn());
                dtCar = new DataTable();
                adtCar.Fill(dtCar);
                dgvCar.DataSource = dtCar;
            }
            catch (Exception)
            {
            }
            dl.close();
        }

        private void btSaveCar_Click(object sender, EventArgs e)
        {
            Boolean isUpdate = false;
            if (tbCarTeamId.Text == "" || cbbCarTeamName.SelectedIndex == -1)
            {
                MessageBox.Show("กรุณาเลือกชื่อทีม", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else {
                //หาว่า id ซ้ำหรือไม่ ถ้าซ้ำ update ถ้าไม่มี insert
                //sql
                OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                pgCommand.CommandText = "SELECT รหัสรถร่วม FROM public.base_car where รหัสรถร่วม = '" + tbCarId.Text + "' AND รหัสทีม  = '" + tbCarTeamId.Text + "'";
                try
                {
                    dl.connect();
                    OdbcDataReader reader = pgCommand.ExecuteReader();
                    isUpdate = reader.Read();
                }
                catch (Exception)
                {
                }
                dl.close();

                //update or save
                if (isUpdate)
                    updateBaseCarAction();
                else
                    saveBaseCarAction();

            }
        }

        private void updateBaseCarAction() {
            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "UPDATE base_car SET ชื่อรถร่วม = '" + tbCarName.Text + "' WHERE รหัสรถร่วม = '" + tbCarId.Text + "' ; ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                /*กรองตาม ทีมรถ*/
                //this.base_carTableAdapter.Fill(this.baseCarDataSet.base_car);
                //this.basecarBindingSource.Filter = string.Format("รหัสทีม = '" + tbCarTeamId.Text + "'");
                setDataSouceForDGVCar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            dl.close();
        }

        private void saveBaseCarAction(){
            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "INSERT INTO base_car (รหัสรถร่วม, ชื่อรถร่วม, รหัสทีม)" +
                                     "VALUES ('" + tbCarId.Text + "','" + tbCarName.Text + "','" + tbCarTeamId.Text + "' )";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                MessageBox.Show("บันทึกเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                /*กรองตาม ทีมรถ*/
                //this.base_carTableAdapter.Fill(this.baseCarDataSet.base_car);
                //this.basecarBindingSource.Filter = string.Format("รหัสทีม = '" + tbCarTeamId.Text + "'");
                setDataSouceForDGVCar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("รหัสรถร่วมนี้มีอยู่แล้ว กรุณาเปลี่ยนรหัสรถร่วมใหม่", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            dl.close();
        }

        private void btClearCar_Click(object sender, EventArgs e)
        {
            tbCarId.Text = "";
            tbCarId.ReadOnly = false;
            tbCarName.Text = "";
        }

        private void btDelCar_Click(object sender, EventArgs e)
        {
            if (tbCarId.Text == "") {
                MessageBox.Show("กรุณาเลือกรายการที่ต้องการลบ", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else {
                if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //sql
                    OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
                    pgCommand.CommandText = "DELETE FROM base_car WHERE รหัสรถร่วม = '" + tbCarId.Text + "' ; ";
                    try
                    {
                        dl.connect();
                        OdbcDataReader reader = pgCommand.ExecuteReader();
                        MessageBox.Show("ลบรายการเรียบร้อย", "บันทึก", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        /*กรองตาม ทีมรถ*/
                        //this.base_carTableAdapter.Fill(this.baseCarDataSet.base_car);
                        //this.basecarBindingSource.Filter = string.Format("รหัสทีม = '" + tbCarTeamId.Text + "'");
                        setDataSouceForDGVCar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    dl.close();
                }
            }
        }

        private void cbbCarTeamName_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbCarId.Text = "";
            tbCarName.Text = "";

            //sql
            OdbcCommand pgCommand = (OdbcCommand)dl.sqlConn().CreateCommand();
            pgCommand.CommandText = "SELECT รหัสทีม FROM public.base_car_team where ชื่อทีม = '" + cbbCarTeamName.Text + "' ";
            try
            {
                dl.connect();
                OdbcDataReader reader = pgCommand.ExecuteReader();
                while (reader.Read())
                {
                    string rdStr = reader["รหัสทีม"].ToString();
                    tbCarTeamId.Text = rdStr;
                }
            }
            catch (Exception)
            {
            }
            dl.close();

            /*กรองตาม ทีมรถ*/
            //this.basecarBindingSource.Filter = string.Format("รหัสทีม = '" + tbCarTeamId.Text + "'");
            setDataSouceForDGVCar();

            /*set ค่าที่มาจาก Table base_car_team*/
            setDataCarTeam();
        }

        private void btDelDriver_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //basecarteamBindingSource.RemoveCurrent();
                //saveActionCarTeam();

                deleteDTGV(dgvDriver, "รหัสผู้ขับ", "base_driver");
            }
        }

        private void btSaveDriver_Click(object sender, EventArgs e)
        {
            saveAndUpdateDTGV(cmbDriver, adtDriver, dtDriver, dgvDriver, "base_driver");
        }

        private void btDelCarRegistration_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("ต้องการลบรายการนี้ใช่หรือไม่", "แจ้งเตือน", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //basecarteamBindingSource.RemoveCurrent();
                //saveActionCarTeam();

                deleteDTGV(dgvCarRegistration, "รหัสทะเบียนรถ", "base_car_registration");
            }
        }

        private void btSaveCarRegistration_Click(object sender, EventArgs e)
        {
            saveAndUpdateDTGV(cmbCarRegistration, adtCarRegistration, dtCarRegistration, dgvCarRegistration, "base_car_registration");
        }
    }
}
