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

        public ucHelp()
        {
            InitializeComponent();
            this.Disposed += UcHelp_Disposed;
        }

        private void UcHelp_Disposed(object sender, EventArgs e)
        {
            if (_spManager != null)
            {
                _spManager.NewSerialDataRecieved -= _spManager_NewSerialDataRecieved;
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

            // Bind Parity ComboBox
            cboParity.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            cboParity.SelectedItem = settings.Parity;

            // Bind DataBits ComboBox
            cboDataBits.DataSource = settings.DataBitsCollection;
            cboDataBits.SelectedItem = settings.DataBits;

            // Bind StopBits ComboBox
            cboStopBits.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));
            cboStopBits.SelectedItem = settings.StopBits;

            // Wire UI changes to update settings
            cboPort.SelectedIndexChanged += (s, ev) => { settings.PortName = cboPort.Text; };
            cboBaud.SelectedIndexChanged += (s, ev) => { if (cboBaud.SelectedItem != null) settings.BaudRate = (int)cboBaud.SelectedItem; };
            cboParity.SelectedIndexChanged += (s, ev) => { if (cboParity.SelectedItem != null) settings.Parity = (System.IO.Ports.Parity)cboParity.SelectedItem; };
            cboDataBits.SelectedIndexChanged += (s, ev) => { if (cboDataBits.SelectedItem != null) settings.DataBits = (int)cboDataBits.SelectedItem; };
            cboStopBits.SelectedIndexChanged += (s, ev) => { if (cboStopBits.SelectedItem != null) settings.StopBits = (System.IO.Ports.StopBits)cboStopBits.SelectedItem; };

            // Subscribe to new incoming data
            _spManager.NewSerialDataRecieved += _spManager_NewSerialDataRecieved;
        }

        private void _spManager_NewSerialDataRecieved(object sender, SerialPortListener.Serial.SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<SerialPortListener.Serial.SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 2000; // maximum text length in text box
            if (txtDataReceived.TextLength > maxTextLength)
                txtDataReceived.Text = txtDataReceived.Text.Remove(0, txtDataReceived.TextLength - maxTextLength);

            string str = System.Text.Encoding.ASCII.GetString(e.Data);
            txtDataReceived.AppendText(str);
            txtDataReceived.ScrollToCaret();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (_spManager != null)
            {
                try
                {
                    _spManager.StartListening();
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
