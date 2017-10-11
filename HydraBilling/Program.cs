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

namespace HydraBilling
{
    class Program
    {
        static void Main(string[] args)
        {
            OracleConnection conn;
            HttpServer httpServer;
            Thread thread;

            log4net.Config.XmlConfigurator.Configure();

            var route_config = new List<SimpleHttpServer.Models.Route>() {
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

            conn = DBUtils.GetDBConnection();
            conn.StateChange += Conn_StateChange;
            MyConsole.Info("Oracle: get connection: " + conn);
            try
            {
                conn.Open();
                MyConsole.Info("Oracle: connection successful");

                MyConsole.Info("Hydra billing: init start");
                HydraBilling.Context.Init(conn);
                MyConsole.Info("Hydra billing: init complite");
            }
            catch (Exception ex)
            {
                MyConsole.Error(ex.Message);
                Console.Read();
                return;
            }         

            MyConsole.Info("Web server: starting");
            httpServer = new HttpServer(8081, route_config);
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
        }

        private static void Conn_StateChange(object sender, StateChangeEventArgs e)
        {
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
                ContentAsUTF8 = "Hello from SimpleHttpServer",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }

        private static HttpResponse RunProcedure(HttpRequest request)
        {
            string command = request.Path;
            JObject data = JObject.Parse(request.Content);

            string resultData = string.Empty;

            switch (command)
            {

                case "Comments/Put":
                    resultData = HydraBilling.Context.Comments.Put(data["UserID"].Value<int>(), data["Message"].Value<string>());
                    break;
            }

            return new HttpResponse()
            {
                ContentAsUTF8 = resultData,
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }
    }
}
