using Nancy.Hosting.Self;
using System;

namespace WinAppDriver.Server
{
    public class Host : IDisposable
    {
        private readonly NancyHost _nancyHost;

        public Host(Uri uri)
        {
            var hostConfigs = new HostConfiguration
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
                //RewriteLocalhost = false
            };

            _nancyHost = new NancyHost(new Bootstrapper(), hostConfigs, uri);
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
