using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.Data;

namespace HydraBilling
{
    class DBUtils
    {
        public static OracleConnection GetDBConnection()
        {
            string host = "46.101.123.246";
            int port = 1521;
            string sid = "hydra";
            string user = "AIS_RPC";
            string password = "TcNzS2bfxwyVc";

            return DBOracleUtils.GetDBConnection(host, port, sid, user, password);
        }
    }
}
