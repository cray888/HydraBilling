using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Threading;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.RouteHandlers;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data.Common;

namespace HydraBilling
{
    class Program
    {
        static void Main(string[] args)
        {
            int webServerPort = 8081;
            string oracleHost = "46.101.123.246";
            int oraclePort = 1521;
            string oracleSid = "hydra";
            string oracleUser = "AIS_RPC";
            string oraclePassword = "TcNzS2bfxwyVc";

            OracleConnection conn;
            HttpServer httpServer;
            Thread threadWD;
            Thread thread;

            log4net.Config.XmlConfigurator.Configure();

            var route_config = new List<Route>() {
                new Route {
                    Name = "Run query",
                    UrlRegex = @"^/Query/(.*)$",
                    Method = "POST",
                    Callable = RunQuery
                },
                new Route {
                    Name = "Run procedure",
                    UrlRegex = @"^/Procedure/(.*)$",
                    Method = "POST",
                    Callable = RunProcedure
                },
            };

            conn = DBUtils.GetDBConnection(oracleHost, oraclePort, oracleSid, oracleUser, oraclePassword);
            //conn.StateChange += Conn_StateChange;
            MyConsole.Info("Oracle: get connection: " + conn);
            try
            {
                conn.Open();
                MyConsole.Info("Oracle: connection successful");

                MyConsole.Info("Hydra billing: init start");
                HydraBilling.Context.Init(conn);
                MyConsole.Info("Hydra billing: init complite");
                conn.Close();
            }
            catch (Exception ex)
            {
                MyConsole.Error(ex.Message);
                Console.Read();
                return;
            }

            DBWatchDog dbWatchDog = new DBWatchDog(conn);
            threadWD = new Thread(new ThreadStart(dbWatchDog.Process));
            //threadWD.Start();
            
            MyConsole.Info("Web server: starting");
            httpServer = new HttpServer(webServerPort, route_config);
            thread = new Thread(new ThreadStart(httpServer.Listen));
            thread.Start();
            
            Thread.Sleep(100);
            if (!thread.IsAlive)
            { 
                MyConsole.Error(httpServer.error);
                Console.Read();
                return;
            }
            MyConsole.Info("Web server: started");



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
            }
            using (DbDataReader dbReader = DBOracleUtils.OracleQuery("select VC_SUBJ_NAME from SI_V_USERS", conn))
            {
                while (dbReader.Read())
                {
                    int empNameIndex = dbReader.GetOrdinal("VC_SUBJ_NAME");// 2
                    string empName = dbReader.GetString(empNameIndex);
                    Console.WriteLine("VC_SUBJ_NAME:" + empName);
                }
            }*/
        }

        private static void Conn_StateChange(object sender, StateChangeEventArgs e)
        {
            MyConsole.Info(e.CurrentState.ToString());

            if (e.CurrentState == ConnectionState.Broken || e.CurrentState == ConnectionState.Closed)
            {
                MyConsole.Error("Oracle: connection lost");
                try
                {
                    OracleConnection conn = (OracleConnection)sender;
                    conn.Open();
                }
                catch (Exception ex)
                {
                    MyConsole.Error(ex.Message);
                    return;
                }
                MyConsole.Info("Oracle: connection successful");
            }
        }

        private static HttpResponse RunQuery(HttpRequest request)
        {
            return new HttpResponse()
            {
                ContentAsUTF8 = "JSON query result",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }

        private static HttpResponse RunProcedure(HttpRequest request)
        {
            string command = request.Path;
            JObject data = JObject.Parse(request.Content);

            string resultData = string.Empty;
            string StatusCode = "200";

            switch (command)
            {

                case "Comments/Put":
                    resultData = HydraBilling.Context.Comments.Put(data["UserID"].Value<int>(), data["Message"].Value<string>());
                    break;
                default:
                    resultData = "Procedure not found!";
                    StatusCode = "405";
                    break;
            }

            return new HttpResponse()
            {
                ContentAsUTF8 = resultData,
                ReasonPhrase = "OK",
                StatusCode = StatusCode
            };
        }
    }
}
