using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SerialPortListener
{
    public partial class ucHelp : UserControl
    {
        private static ucHelp _instance;
        private static ucHelp Instance
        {

            get
            {
                if (_instance == null)
                    _instance = new ucHelp();
                return _instance;
            }
        }
        private SerialPortListener.Serial.SerialPortManager _spManager;

        private readonly object _rxLock = new object();
        private StringBuilder _rxBuffer = new StringBuilder();
        private const int MaxRxTextLength = 2000;

        // Program Files (ที่ติดตั้งโปรแกรม) เขียนไฟล์ไม่ได้ถ้าไม่ใช่ admin จึงเก็บ config ไว้ใน AppData ของผู้ใช้แทน
        // AppDataDir is per-build (see Utils.AppDataDir) so Blue and Pink never share the same config file.
        private static readonly string AppDataDir = Utils.AppDataDir;
        private static readonly string PortConfigPath =
            System.IO.Path.Combine(AppDataDir, "config_port.txt");

        // Adapted from Master_Blue_1 (b02df198): previously only the port name was persisted;
        // Baud/Parity/DataBits/StopBits reset to SerialSettings' hardcoded defaults on every
        // launch. Stored as simple Key=Value lines, unknown/missing/invalid entries fall back
        // to whatever SerialSettings already has (its own hardcoded defaults), never Blue's.
        private static readonly string SerialConfigPath =
            System.IO.Path.Combine(AppDataDir, "config_serial.txt");

        // เก็บรูปแบบข้อมูลตาชั่งที่เลือกไว้ (ดู WeightFormat.cs) ไฟล์เดียวเก็บชื่อ enum บรรทัดเดียว
        private static readonly string WeightFormatConfigPath =
            System.IO.Path.Combine(AppDataDir, "config_weightformat.txt");

        public ucHelp()
        {
            InitializeComponent();
            this.Disposed += UcHelp_Disposed;

            cboWeightFormat.DataSource = WeightFormatCatalog.All;
            MainForm.CurrentWeightFormat = LoadSavedWeightFormat();
            SelectWeightFormatInCombo(MainForm.CurrentWeightFormat);
            tbWeightPreview.Text = "-  -  -";
        }

        private void UcHelp_Disposed(object sender, EventArgs e)
        {
            if (_spManager != null)
            {
                _spManager.NewSerialDataRecieved -= _spManager_NewSerialDataRecieved;
            }
        }

        // ตอน constructor ทำงาน (สร้าง ucHelp เป็นลูกของ MainForm) ยังไม่ผ่าน Login
        // Globals.Permission จึงยังไม่ถูกตั้งค่า เช็คสิทธิ์ใหม่ทุกครั้งที่แสดงหน้านี้แทน
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
                ApplyPortConfigPermission();
        }

        // เฉพาะ user ที่มีสิทธิ์ add_setting เท่านั้นที่แก้ไข/บันทึกพอร์ตได้ user อื่นดูได้อย่างเดียว
        private void ApplyPortConfigPermission()
        {
            bool canEdit = Globals.isPermissionAddSetting();

            cboPort.Enabled = canEdit;
            cboBaud.Enabled = canEdit;
            cboParity.Enabled = canEdit;
            cboDataBits.Enabled = canEdit;
            cboStopBits.Enabled = canEdit;
            btnSavePort.Visible = canEdit;
            btnSavePort.Enabled = canEdit;

            // การเลือกรูปแบบตาชั่งต้องมีสิทธิ์ แต่ตัวเลขน้ำหนักที่อ่านได้ (tbWeightPreview) แสดงให้ทุกคนเห็นได้เสมอ
            cboWeightFormat.Enabled = canEdit;
        }

        private WeightFormat SelectedWeightFormat()
        {
            var option = cboWeightFormat.SelectedItem as WeightFormatCatalog.Option;
            return option != null ? option.Format : WeightFormat.ParenCR;
        }

        // เลือกรายการใน combo ให้ตรงกับ format ที่ระบุ (ใช้ตอนโหลดค่าที่บันทึกไว้)
        private void SelectWeightFormatInCombo(WeightFormat format)
        {
            foreach (WeightFormatCatalog.Option option in cboWeightFormat.Items)
            {
                if (option.Format == format)
                {
                    cboWeightFormat.SelectedItem = option;
                    return;
                }
            }
        }

        // เลือกรูปแบบใน combo แล้วมีผลทันที (แบบเดียวกับ baud/parity/databits/stopbits)
        // และคำนวณ tbWeightPreview ใหม่ทันทีจากข้อมูลดิบที่มีอยู่แล้ว ไม่ต้องรอ timerRx tick ถัดไป
        private void cboWeightFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainForm.CurrentWeightFormat = SelectedWeightFormat();
            RefreshWeightPreview();
        }

        // อ่านค่าน้ำหนักล่าสุดจากข้อมูลดิบที่มีอยู่แล้วในกล่องตรวจสอบ ด้วย parser/รูปแบบที่เลือกอยู่ตอนนี้
        // ใช้ทั้งตอนข้อมูลใหม่เข้ามา (timerRx_Tick) และตอนเปลี่ยนรูปแบบ (cboWeightFormat_SelectedIndexChanged)
        private void RefreshWeightPreview()
        {
            try
            {
                string parsed = MainForm.ParseWeightFromBuffer(txtDataReceived.Text, tbWeightPreview.Text, SelectedWeightFormat(), out _);
                if (parsed != null)
                    tbWeightPreview.Text = parsed;
            }
            catch (Exception)
            {
            }
        }

        // อ่านรูปแบบที่บันทึกไว้จาก config_weightformat.txt คืนค่า default (ParenCR) ถ้าไม่มีไฟล์/อ่านไม่ได้/ค่าที่บันทึกไว้ไม่รู้จัก
        private static WeightFormat LoadSavedWeightFormat()
        {
            try
            {
                if (!System.IO.File.Exists(WeightFormatConfigPath))
                    return WeightFormat.ParenCR;

                string[] lines = System.IO.File.ReadAllLines(WeightFormatConfigPath);
                if (lines.Length > 0 && Enum.TryParse(lines[0].Trim(), out WeightFormat saved))
                    return saved;
            }
            catch (Exception)
            {
            }
            return WeightFormat.ParenCR;
        }

        private void SaveWeightFormat()
        {
            System.IO.File.WriteAllLines(WeightFormatConfigPath, new[] { SelectedWeightFormat().ToString() });
        }

        // อ่านค่า COM port ที่บันทึกไว้จาก config_port.txt (บรรทัดเดียว เช่น "COM4") ถ้าไม่มีไฟล์หรืออ่านไม่ได้คืนค่า null
        private static string LoadSavedPort()
        {
            try
            {
                if (!System.IO.File.Exists(PortConfigPath))
                    return null;

                string[] lines = System.IO.File.ReadAllLines(PortConfigPath);
                return lines.Length > 0 ? lines[0].Trim() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // อ่านค่า Baud/Parity/DataBits/StopBits ที่บันทึกไว้จาก config_serial.txt
        // คืนค่า null ถ้าไม่มีไฟล์หรืออ่านไม่ได้ ค่าที่ parse ไม่ได้จะถูกข้าม (ใช้ค่า default ของ SerialSettings แทน)
        private static Dictionary<string, string> LoadSavedSerialSettings()
        {
            try
            {
                if (!System.IO.File.Exists(SerialConfigPath))
                    return null;

                var result = new Dictionary<string, string>();
                foreach (string line in System.IO.File.ReadAllLines(SerialConfigPath))
                {
                    int idx = line.IndexOf('=');
                    if (idx <= 0) continue;
                    result[line.Substring(0, idx).Trim()] = line.Substring(idx + 1).Trim();
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void ApplySavedSerialSettings(SerialPortListener.Serial.SerialSettings settings)
        {
            var saved = LoadSavedSerialSettings();
            if (saved == null) return;

            if (saved.TryGetValue("Baud", out string baudStr) && int.TryParse(baudStr, out int baud)
                && settings.BaudRateCollection.Contains(baud))
            {
                cboBaud.SelectedItem = baud;
            }
            if (saved.TryGetValue("Parity", out string parityStr)
                && Enum.TryParse(parityStr, out System.IO.Ports.Parity parity))
            {
                cboParity.SelectedItem = parity;
            }
            if (saved.TryGetValue("DataBits", out string dataBitsStr) && int.TryParse(dataBitsStr, out int dataBits)
                && Array.IndexOf(settings.DataBitsCollection, dataBits) >= 0)
            {
                cboDataBits.SelectedItem = dataBits;
            }
            if (saved.TryGetValue("StopBits", out string stopBitsStr)
                && Enum.TryParse(stopBitsStr, out System.IO.Ports.StopBits stopBits))
            {
                cboStopBits.SelectedItem = stopBits;
            }
        }

        private void SaveSerialSettings()
        {
            var lines = new[]
            {
                "Baud=" + cboBaud.SelectedItem,
                "Parity=" + cboParity.SelectedItem,
                "DataBits=" + cboDataBits.SelectedItem,
                "StopBits=" + cboStopBits.SelectedItem,
            };
            System.IO.File.WriteAllLines(SerialConfigPath, lines);
        }

        private void btnSavePort_Click(object sender, EventArgs e)
        {
            if (!Globals.isPermissionAddSetting())
            {
                MessageBox.Show("คุณไม่มีสิทธิ์บันทึกการตั้งค่านี้", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboPort.SelectedItem == null)
            {
                MessageBox.Show("Please select a COM port.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (!System.IO.Directory.Exists(AppDataDir))
                    System.IO.Directory.CreateDirectory(AppDataDir);
                System.IO.File.WriteAllLines(PortConfigPath, new[] { cboPort.SelectedItem.ToString() });
                SaveSerialSettings();
                SaveWeightFormat();
                MessageBox.Show("บันทึกการตั้งค่าสำเร็จ", "Port", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("บันทึกการตั้งค่าไม่สำเร็จ: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SetSerialPortManager(SerialPortListener.Serial.SerialPortManager spManager)
        {
            _spManager = spManager;
            if (_spManager == null) return;

            var settings = _spManager.CurrentSerialSettings;

            // Ensure baud rate collection has items
            if (settings.BaudRateCollection == null || settings.BaudRateCollection.Count == 0)
            {
                foreach (int rate in new int[] { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200 })
                {
                    settings.BaudRateCollection.Add(rate);
                }
            }

            // Ensure PortNameCollection is populated
            if (settings.PortNameCollection == null || settings.PortNameCollection.Length == 0)
            {
                settings.PortNameCollection = System.IO.Ports.SerialPort.GetPortNames();
            }

            // Populate Port ComboBox
            cboPort.Items.Clear();
            if (settings.PortNameCollection != null)
            {
                cboPort.Items.AddRange(settings.PortNameCollection);
                if (!string.IsNullOrEmpty(settings.PortName) && cboPort.Items.Contains(settings.PortName))
                {
                    cboPort.SelectedItem = settings.PortName;
                }
                else if (cboPort.Items.Count > 0)
                {
                    cboPort.SelectedIndex = 0;
                    settings.PortName = cboPort.Text;
                }
            }

            // Bind Baud ComboBox
            cboBaud.DataSource = settings.BaudRateCollection;
            if (settings.BaudRate > 0)
            {
                cboBaud.SelectedItem = settings.BaudRate;
            }

            // config_port.txt เก็บพอร์ตที่บันทึกไว้ล่าสุด ถ้ามีไฟล์นี้ให้ใช้แทนค่าจาก _spManager
            string savedPort = LoadSavedPort();
            if (!string.IsNullOrEmpty(savedPort) && cboPort.Items.Contains(savedPort))
            {
                cboPort.SelectedItem = savedPort;
                // cboPort.SelectedIndexChanged isn't wired up yet at this point (see below),
                // so settings.PortName must be synced here explicitly or it stays stale.
                settings.PortName = savedPort;
            }

            // Bind Parity ComboBox
            cboParity.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            cboParity.SelectedItem = settings.Parity;

            // Bind DataBits ComboBox
            cboDataBits.DataSource = settings.DataBitsCollection;
            cboDataBits.SelectedItem = settings.DataBits;

            // Bind StopBits ComboBox
            cboStopBits.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));
            cboStopBits.SelectedItem = settings.StopBits;

            // config_serial.txt เก็บ Baud/Parity/DataBits/StopBits ที่บันทึกไว้ล่าสุด
            // (ก่อนหน้านี้ยังไม่มีการบันทึกค่าเหล่านี้ จะรีเซ็ตเป็นค่า default ทุกครั้งที่เปิดโปรแกรม)
            ApplySavedSerialSettings(settings);
            settings.BaudRate = (int)cboBaud.SelectedItem;
            settings.Parity = (System.IO.Ports.Parity)cboParity.SelectedItem;
            settings.DataBits = (int)cboDataBits.SelectedItem;
            settings.StopBits = (System.IO.Ports.StopBits)cboStopBits.SelectedItem;

            // Wire UI changes to update settings
            cboPort.SelectedIndexChanged += (s, ev) => { settings.PortName = cboPort.Text; };
            cboBaud.SelectedIndexChanged += (s, ev) => { if (cboBaud.SelectedItem != null) settings.BaudRate = (int)cboBaud.SelectedItem; };
            cboParity.SelectedIndexChanged += (s, ev) => { if (cboParity.SelectedItem != null) settings.Parity = (System.IO.Ports.Parity)cboParity.SelectedItem; };
            cboDataBits.SelectedIndexChanged += (s, ev) => { if (cboDataBits.SelectedItem != null) settings.DataBits = (int)cboDataBits.SelectedItem; };
            cboStopBits.SelectedIndexChanged += (s, ev) => { if (cboStopBits.SelectedItem != null) settings.StopBits = (System.IO.Ports.StopBits)cboStopBits.SelectedItem; };

            // Subscribe to new incoming data
            _spManager.NewSerialDataRecieved += _spManager_NewSerialDataRecieved;

            ApplyPortConfigPermission();
        }

        // Runs on the SerialPort's background thread. Only buffers data - no UI access here,
        // so a burst of fast-arriving data can't flood the UI thread's Invoke queue.
        private void _spManager_NewSerialDataRecieved(object sender, SerialPortListener.Serial.SerialDataEventArgs e)
        {
            try
            {
                string str = System.Text.Encoding.ASCII.GetString(e.Data);
                lock (_rxLock)
                {
                    _rxBuffer.Append(str);
                }
            }
            catch (Exception)
            {
            }
        }

        // Runs on the UI thread on a fixed interval, draining whatever arrived since the last
        // tick in one go, instead of once per DataReceived event.
        private void timerRx_Tick(object sender, EventArgs e)
        {
            string pending;
            lock (_rxLock)
            {
                if (_rxBuffer.Length == 0)
                    return;
                pending = _rxBuffer.ToString();
                _rxBuffer.Clear();
            }

            try
            {
                txtDataReceived.AppendText(pending);
                if (txtDataReceived.TextLength > MaxRxTextLength)
                    txtDataReceived.Text = txtDataReceived.Text.Remove(0, txtDataReceived.TextLength - MaxRxTextLength);
                txtDataReceived.SelectionStart = txtDataReceived.Text.Length;
                txtDataReceived.ScrollToCaret();

                // แสดงน้ำหนักที่อ่านได้แบบสด โดยใช้ parser ตัวเดียวกับที่ MainForm ใช้จริง
                // กับรูปแบบที่เลือกอยู่ใน cboWeightFormat ตอนนี้ ไม่ต้องพิมพ์ข้อมูลทดสอบเอง
                RefreshWeightPreview();
            }
            catch (Exception)
            {
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_spManager != null)
            {
                try
                {
                    // Belt-and-suspenders: make sure the port actually opened matches what's
                    // shown on screen, regardless of when the combo's change handlers were wired.
                    if (cboPort.SelectedItem != null)
                        _spManager.CurrentSerialSettings.PortName = cboPort.SelectedItem.ToString();

                    _spManager.StartListening();
                    timerRx.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error starting COM port: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_spManager != null)
            {
                try
                {
                    _spManager.StopListening();
                    timerRx.Stop();
                    tbWeightPreview.Text = "-  -  -";
                    ApplyPortConfigPermission();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error stopping COM port: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ucHelp_Load(object sender, EventArgs e)
        {

        }
    }
}
