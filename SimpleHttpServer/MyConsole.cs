using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    class MyConsole
    {
        static public void Info(string text)
        {
            WriteConsole("INF", text, ConsoleColor.Green);
        }

        static public void Error(string text)
        {
            WriteConsole("ERR", text, ConsoleColor.Red);
        }

        static private void WriteConsole(string prefix, string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(String.Format("{0} {1}", DateTime.Now, prefix));
            Console.ResetColor();
            Console.WriteLine(String.Format(" - {0}", text));
        }
    }
}
