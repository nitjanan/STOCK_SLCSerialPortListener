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
        FixedOffsetTail,
        PlainPadded,
        JobOutQMarker,
        SrdStxSigned,
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
            // [..] ท้ายชื่อ = branch ที่เคย hardcode รูปแบบนี้ไว้
            new Option(WeightFormat.ParenCR, "แบบ ( ... CR (ค่าเริ่มต้น) [T3, T4, JOB ขาเข้า, KRD, TYM เดิม]"),
            new Option(WeightFormat.QMarker, "แบบ q ... CR [CTM, T3 เดิม]"),
            new Option(WeightFormat.JobOutQMarker, "แบบ q ... ETB [JOB ขาออก]"),
            new Option(WeightFormat.PMarker, "แบบ p ... CR [T5, M3]"),
            new Option(WeightFormat.PQDual, "แบบ p/q (มีเครื่องหมาย) [39]"),
            new Option(WeightFormat.KNTerminated, "แบบลงท้าย KN [KT]"),
            new Option(WeightFormat.KgTerminated, "แบบลงท้าย kg [UNI]"),
            new Option(WeightFormat.NsmCsv, "แบบ ST,GS,...,Kg [NSM]"),
            new Option(WeightFormat.SrdSigned, "แบบมีเครื่องหมายลบ [SRD]"),
            new Option(WeightFormat.SrdStxSigned, "แบบ STX ... CR มีเครื่องหมายลบ [SRD_Blue_11/03/25]"),
            new Option(WeightFormat.TymFixedWidth, "แบบฟิลด์คงที่ *0 (12 หลัก) [TYM]"),
            new Option(WeightFormat.FixedOffsetTail, "แบบตำแหน่งคงที่ท้ายบัฟเฟอร์ (ของเดิม/สำรอง) [ทุก branch เก่า]"),
            new Option(WeightFormat.PlainPadded, "แบบตัวเลขล้วน ลงท้าย U = ยังไม่นิ่ง (เช่น \"    1930 \") [SRD ผลิต]"),
        };
    }
}
