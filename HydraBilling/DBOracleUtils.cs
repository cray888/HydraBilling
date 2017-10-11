using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

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
    }
}
