using WinAppDriver.Server;
using System;

namespace ClientApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] args)
        {
            var uri = new Uri("http://localhost:4444");

            if (args.Length == 1)
            {
                if (!Uri.TryCreate(args[0], UriKind.Absolute, out uri))
                {
                    Console.Out.WriteLine($"Provided value '{args[0]}' is not a valid URI.");
                    return;
                }
            }

            var commandDispatcher = new DriverHost(uri);
            commandDispatcher.Start();
            Console.ReadLine();
        }
    }
}
