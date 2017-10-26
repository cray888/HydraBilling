using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data.Common;
using System.IO;
using System.Xml;

namespace HydraBilling
{
    class DBOracleUtils
    {
        public static OracleConnection
                       GetDBConnection(string host, int port, String sid, String user, String password)
        {
            MyConsole.Info("Oracle: getting connection");
            // 'Connection string' to connect directly to Oracle.
            string connString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
                 + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
                 + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connString;
            return conn;
        }

        public static DbDataReader OracleQuery(string query, OracleConnection conn)
        {
            conn.Open();
            OracleCommand cmd = new OracleCommand(query, conn);
            /*using (DbDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                     
                    while (reader.Read())
                    {
                        // Get index of Column Emp_ID in query statement.
                        int empIdIndex = reader.GetOrdinal("Emp_Id"); // 0
                         
 
                        long empId =  Convert.ToInt64(reader.GetValue(0));
                         
                        // Index of Emp_ID = 1
                        string empNo = reader.GetString(1);
                        int empNameIndex = reader.GetOrdinal("Emp_Name");// 2
                        string empName = reader.GetString(empNameIndex);
 
                        // Index of column Mng_Id.
                        // Chỉ số (index) của cột Mng_Id trong câu lệnh SQL.
                        int mngIdIndex = reader.GetOrdinal("Mng_Id");
 
                        long? mngId = null;
  
                        if (!reader.IsDBNull(mngIdIndex))
                        {
                            mngId = Convert.ToInt64(reader.GetValue(mngIdIndex)); 
                        }
                        Console.WriteLine("--------------------");
                        Console.WriteLine("empIdIndex:" + empIdIndex);
                        Console.WriteLine("EmpId:" + empId);
                        Console.WriteLine("EmpNo:" + empNo);
                        Console.WriteLine("EmpName:" + empName);
                        Console.WriteLine("MngId:" + mngId);
                    }
                }
            }*/
            cmd.BindByName = true;
            //cmd.XmlCommandType = OracleXmlCommandType.Query;
            //XmlReader xmlReader = cmd.ExecuteXmlReader();
            conn.Close();
            return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
    }
}
