using WinAppDriver.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var commandDispatcher = new CommandDispatcher("http://localhost:12345");
            commandDispatcher.Start();
            Console.ReadLine();
        }
    }
}
