using System;

namespace SerialPortListener
{
    // การตั้งค่าทั่วไปของหน้า MainForm ที่ไม่ผูกกับตารางในฐานข้อมูล เก็บเป็นไฟล์ config
    // ใน AppData แบบเดียวกับ config_port.txt/config_serial.txt/config_weightformat.txt/config_report_logo.txt
    public static class MainFormSettings
    {
        private static readonly string AutoFillWeightInConfigPath =
            System.IO.Path.Combine(Utils.AppDataDir, "config_autofill_weightin.txt");

        // ค่าเริ่มต้น = ไม่ดึงน้ำหนักเข้าล่าสุดของรถอัตโนมัติ (getWeightInOnDay จะไม่ถูกเรียก)
        public static bool GetAutoFillWeightInEnabled()
        {
            try
            {
                if (System.IO.File.Exists(AutoFillWeightInConfigPath))
                {
                    string[] lines = System.IO.File.ReadAllLines(AutoFillWeightInConfigPath);
                    if (lines.Length > 0 && bool.TryParse(lines[0].Trim(), out bool saved))
                        return saved;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public static void SetAutoFillWeightInEnabled(bool enabled)
        {
            if (!System.IO.Directory.Exists(Utils.AppDataDir))
                System.IO.Directory.CreateDirectory(Utils.AppDataDir);
            System.IO.File.WriteAllLines(AutoFillWeightInConfigPath, new[] { enabled.ToString() });
        }
    }
}
