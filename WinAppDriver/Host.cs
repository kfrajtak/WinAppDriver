using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinAppDriver.Server
{
    public class Host : IDisposable
    {
        private readonly NancyHost _nancyHost;

        public Host(Uri uri, CommandEnvironment commandEnvironment)
        {
            var hostConfigs = new HostConfiguration
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };

            _nancyHost = new NancyHost(new Bootstrapper(commandEnvironment), hostConfigs, uri);
        }        

        public void Start()
        {
            _nancyHost.Start();
        }

        public void Dispose()
        {
            _nancyHost.Dispose();
        }
    }
}
