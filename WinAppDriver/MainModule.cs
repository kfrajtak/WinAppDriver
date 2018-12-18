using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinAppDriver.Server
{
    public class MainModule : NancyModule
    {
        public MainModule()
        {
            Get["/"] = parameters => Response.AsJson(new[] { new { Name = "Site 1", DeployStatus = "Deployed" } });
        }
    }
}
