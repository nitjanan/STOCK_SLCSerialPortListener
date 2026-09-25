using Devart.Data.PostgreSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;

namespace SerialPortListener
{
    public class Datalayer
    {
        OdbcConnection conn;
        OdbcCommand cmd;
        public string getmassage { get; set; }

        // นับจำนวนครั้งที่ connect() ถูกเรียกซ้อนกัน เพื่อให้เรียก connect()/close() ซ้อนกันได้อย่างปลอดภัย
        // (เช่น เปิดการเชื่อมต่อไว้ครั้งเดียวคร่อมหลายคำสั่ง SQL แทนที่จะเปิด-ปิดใหม่ทุกคำสั่ง) โดยโค้ดเดิมที่เรียก
        // connect()/close() เป็นคู่ๆ อยู่แล้วทุกที่ยังทำงานเหมือนเดิมทุกประการ ไม่กระทบพฤติกรรมเดิม
        private int _connectDepth = 0;

        public Datalayer() {
            //String cs = "User Id=postgres;Host=localhost;Database=truck;Password=postgres;Initial Schema=public;charset=UTF8";
            //String cs = "User Id=sa;Host=192.168.10.132;Database=truck;Password=123456;Initial Schema=public;charset=UTF8";
            //string cs = "DSN=PostgreSQLM";
            string cs = "DSN=PostgreSQLSTOCK";
            //string cs = "DSN=PostgreSQLS";
            //string cs = "DSN=PostgreSQLCenterS";
            //string cs = "DSN=PostgreSQLCenterM";
            //string cs = "DSN=PostgreSQLCenterSTOCK";
            //string cs = "DSN=PostgreSQLCenterALL";
            conn = new OdbcConnection(cs);
            cmd = new OdbcCommand();
        }
        public bool connect() {
            if (_connectDepth > 0)
            {
                // มีคนเปิดการเชื่อมต่อค้างไว้อยู่แล้ว (เรียกซ้อน) ใช้ต่อได้เลยไม่ต้องเปิดใหม่
                _connectDepth++;
                getmassage = "Connect successfully";
                return true;
            }
            try {
                conn.Open();
                _connectDepth = 1;
                getmassage = "Connect successfully";
                return true;
            }
            catch (Exception) {
                getmassage = "Connect fail";
                return false;
            }
        }

        public OdbcConnection sqlConn()
        {
            return conn;
        }
        public bool close()
        {
            if (_connectDepth > 1)
            {
                // ยังมีผู้เรียกอื่นที่เปิดการเชื่อมต่อค้างไว้อยู่ (เรียกซ้อน) ปิดจริงเมื่อตัวนอกสุดปิดเท่านั้น
                _connectDepth--;
                getmassage = "Close successfully";
                return true;
            }
            try
            {
                conn.Close();
                _connectDepth = 0;
                getmassage = "Close successfully";
                return true;
            }
            catch (Exception)
            {
                getmassage = "Close fail";
                return false;
            }
        }
    }
}
