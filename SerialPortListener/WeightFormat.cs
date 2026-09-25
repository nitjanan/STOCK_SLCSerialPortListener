namespace SerialPortListener
{
    // แต่ละสาขา/ลูกค้าของ Stock เดิมมีการ hardcode วิธีอ่านค่าน้ำหนักจากตาชั่งไว้คนละแบบ
    // (แยกกันคนละ branch คนละไฟล์ build) รวบรวมมาเป็นตัวเลือกเดียวที่สลับได้ในโปรแกรมเดียวกัน
    public enum WeightFormat
    {
        ParenCR,
        QMarker,
        PMarker,
        PQDual,
        KNTerminated,
        KgTerminated,
        NsmCsv,
        SrdSigned,
        TymFixedWidth,
    }

    // รายการรูปแบบทั้งหมดสำหรับผูกกับ combo box ตัวเลือกใน ucHelp
    public static class WeightFormatCatalog
    {
        public class Option
        {
            public WeightFormat Format { get; }
            public string Label { get; }

            public Option(WeightFormat format, string label)
            {
                Format = format;
                Label = label;
            }

            public override string ToString() => Label;
        }

        public static readonly Option[] All = new[]
        {
            new Option(WeightFormat.ParenCR, "แบบ ( ... CR (ค่าเริ่มต้น)"),
            new Option(WeightFormat.QMarker, "แบบ q ... CR"),
            new Option(WeightFormat.PMarker, "แบบ p ... CR"),
            new Option(WeightFormat.PQDual, "แบบ p/q (มีเครื่องหมาย)"),
            new Option(WeightFormat.KNTerminated, "แบบลงท้าย KN"),
            new Option(WeightFormat.KgTerminated, "แบบลงท้าย kg"),
            new Option(WeightFormat.NsmCsv, "แบบ ST,GS,...,Kg"),
            new Option(WeightFormat.SrdSigned, "แบบมีเครื่องหมายลบ"),
            new Option(WeightFormat.TymFixedWidth, "แบบฟิลด์คงที่ *0 (12 หลัก)"),
        };
    }
}
