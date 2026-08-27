using System;
using System.Text;

namespace SerialPortListener
{
    internal static class ReportErrorHelper
    {
        public static string BuildMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("เกิดข้อผิดพลาดขณะประมวลผลรายงาน:");
            Exception cur = ex;
            while (cur != null)
            {
                sb.AppendLine(cur.GetType().Name + ": " + cur.Message);
                cur = cur.InnerException;
            }
            return sb.ToString();
        }
    }
}
