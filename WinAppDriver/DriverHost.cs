using System;
using System.Threading;

namespace WinAppDriver.Server
{
    public class DriverHost
    {
        private readonly Uri _uri;
        private Host _host;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public DriverHost(Uri uri)
        {
            _uri = uri;
        }

        public void Start()
        {
            try
            {
                _host = new Host(_uri);
                _host.Start();
                Logger.Info($"Server started at {_uri}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}
