using System;
using System.Collections.Generic;
using System.Threading;
using WinAppDriver.Server;

namespace WinAppDriver.Strategies.SendKeys
{
    public class Grouped : ISendKeyStrategy
    {
        public Response Execute(CommandEnvironment commandEnvironment, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
