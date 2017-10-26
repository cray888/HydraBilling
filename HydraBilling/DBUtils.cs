using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;
using System.Threading;

namespace HydraBilling
{
    class DBUtils
    {
        public static OracleConnection GetDBConnection(string host, int port, string sid, string user, string password)
        {
            return DBOracleUtils.GetDBConnection(host, port, sid, user, password);
        }
    }

    class DBWatchDog
    {
        OracleConnection conn;
        public DBWatchDog(OracleConnection _conn)
        {
            conn = _conn;
        }

        public void Process()
        {
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    if (conn.State == ConnectionState.Open || conn.State == ConnectionState.Executing || conn.State == ConnectionState.Fetching) continue;
                    conn.Open();
                    MyConsole.Info("Oracle: reconnected");
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
