using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WinAppDriver.Extensions;
using WinAppDriver.Input;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class ActionsCommandHandler : CommandHandler
    {
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        public override Response Execute(CommandEnvironment commandEnvironment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            if (!parameters.TryGetValue("actions", out object o))
            {
                return Response.CreateMissingParametersResponse("actions");
            }

            var numberOfRetries = 10;
            Exception e = null;
            while (numberOfRetries-- >= 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                try
                {
                    SetForegroundWindow(commandEnvironment.WindowHandle);
                    commandEnvironment.Window.SetFocus();
                    e = null;
                    break;
                }
                catch (COMException comEx)
                {
                    e = comEx;

                    if (comEx.IsTimeout())
                    {
                        System.Threading.Thread.Sleep(250);
                        continue;
                    }

                    throw;
                }
            }

            if (e != null)
            {
                throw e;
            }

            try
            {
                var strategy = new Strategies.SendKeys.OneByOne();
                return strategy.Execute(commandEnvironment, parameters, cancellationToken);
            }
            finally
            {
                Microsoft.Test.Input.Mouse.Reset();
                Microsoft.Test.Input.Keyboard.Reset();
            }            
        }
    }
}
