using System;

namespace SerialPortListener
{
    // ReportMain.rdlc มีรูปโลโก้ฝังไว้อยู่แล้วหลายรูปจากการพัฒนาที่ผ่านมา (job_logo คือรูปที่ใช้งานอยู่จริง
    // ส่วน logo_old/logo_last/logo_lastremovebgpreview เป็นรูปเก่าที่ยังฝังอยู่ในไฟล์แต่ไม่ได้ใช้งาน)
    // ให้เลือกได้ว่าจะพิมพ์โลโก้ไหนบนใบชั่งน้ำหนัก แทนที่จะต้อง build ใหม่ทุกครั้งที่ต้องการเปลี่ยน
    public enum ReportLogo
    {
        Default,
        Old,
        Last,
        LastRemoveBgPreview,
    }

    public static class ReportLogoSettings
    {
        public class Option
        {
            public ReportLogo Logo { get; }
            public string Label { get; }

            public Option(ReportLogo logo, string label)
            {
                Logo = logo;
                Label = label;
            }

            public override string ToString() => Label;
        }

        public static readonly Option[] All = new[]
        {
            new Option(ReportLogo.Default, "ค่าเริ่มต้น"),
            new Option(ReportLogo.Old, "โลโก้แบบเก่า"),
            new Option(ReportLogo.Last, "โลโก้เวอร์ชันล่าสุด"),
            new Option(ReportLogo.LastRemoveBgPreview, "โลโก้ (พื้นหลังโปร่งใส)"),
        };

        private static readonly string ConfigPath =
            System.IO.Path.Combine(Utils.AppDataDir, "config_report_logo.txt");

        // ชื่อ embedded image ใน ReportMain.rdlc ที่ตรงกับแต่ละตัวเลือก ("" หมายถึงค่าเริ่มต้น/job_logo)
        private static string EmbeddedImageName(ReportLogo logo)
        {
            switch (logo)
            {
                case ReportLogo.Old: return "logo_old";
                case ReportLogo.Last: return "logo_last";
                case ReportLogo.LastRemoveBgPreview: return "logo_lastremovebgpreview";
                case ReportLogo.Default:
                default: return "";
            }
        }

        // ค่าที่ส่งเป็น ReportParameter "PLogoName" ให้ ReportMain.rdlc ("" = ใช้ job_logo ตามค่าเริ่มต้น)
        // อ่านค่าที่บันทึกไว้ ถ้าไม่มีไฟล์/อ่านไม่ได้/ค่าที่บันทึกไว้ไม่รู้จัก จะคืนค่าเริ่มต้นเสมอ ไม่มีวัน throw
        public static string GetSelectedLogoName()
        {
            try
            {
                if (System.IO.File.Exists(ConfigPath))
                {
                    string[] lines = System.IO.File.ReadAllLines(ConfigPath);
                    if (lines.Length > 0 && Enum.TryParse(lines[0].Trim(), out ReportLogo saved))
                        return EmbeddedImageName(saved);
                }
            }
            catch (Exception)
            {
            }
            return EmbeddedImageName(ReportLogo.Default);
        }

        public static ReportLogo GetSelectedLogo()
        {
            try
            {
                if (System.IO.File.Exists(ConfigPath))
                {
                    string[] lines = System.IO.File.ReadAllLines(ConfigPath);
                    if (lines.Length > 0 && Enum.TryParse(lines[0].Trim(), out ReportLogo saved))
                        return saved;
                }
            }
            catch (Exception)
            {
            }
            return ReportLogo.Default;
        }

        public static void SaveSelectedLogo(ReportLogo logo)
        {
            if (!System.IO.Directory.Exists(Utils.AppDataDir))
                System.IO.Directory.CreateDirectory(Utils.AppDataDir);
            System.IO.File.WriteAllLines(ConfigPath, new[] { logo.ToString() });
        }
    }
}
