using Cavalo.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cavalo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new Host(new Uri("http://localhost:12345")))
            {
                host.Start();
                System.Console.WriteLine("Running on http://localhost:12345");
                System.Console.ReadLine();
            }
        }
    }
}
