﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydraBilling
{
    class MyConsole
    {
        static public void Info(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(DateTime.Now + " INF");
            Console.ResetColor();
            Console.Write(" - ");
            Console.WriteLine(text);
        }

        static public void Error(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(DateTime.Now + " ERR");
            Console.ResetColor();
            Console.Write(" - ");
            Console.WriteLine(text);
        }
    }
}
