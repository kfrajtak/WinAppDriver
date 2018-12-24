using System;
using System.Threading;

namespace WinAppDriver.Server
{
    public class DriverHost
    {
        private readonly Uri _uri;
        private Host _host;
        private readonly ManualResetEvent _manualResetEvent;

        public DriverHost(string uri)
        {
            _uri = new Uri(uri);
        }

        public void Start()
        {
            try
            {
                _host = new Host(_uri);
                _host.Start();
                Console.WriteLine($"Running on {_uri}");
                _manualResetEvent.WaitOne();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        public void Stop()
        {
            _manualResetEvent.Set();
        }
    }
}
