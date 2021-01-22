using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using WinAppDriver.CommandHandlers.Helpers;
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
                    //System.Windows.Automation.AutomationElement.FromHandle(commandEnvironment.WindowHandle).SetFocus();
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
                AutomationElement element = null;
                foreach (var block in (JArray)o)
                {
                    var actions = (JArray)block["actions"];
                    var type = block["type"].Value<string>();

                    switch (type)
                    {
                        case "pointer":
                            new MouseActions(actions, commandEnvironment).Execute(out element);
                            break;
                        case "key":
                            if (ActionsHelper.TryGetPlainString(actions, out var input, out var lastKey) && element != null)
                            {
                                element.SetText(input);
                                element = null; // to avoid further problems
                                if (lastKey != null)
                                {
                                    new KeyboardActions(new JArray(actions.Skip(actions.Count() - 2).Take(2)), commandEnvironment).Execute();
                                }
                                break;
                            }
                            new KeyboardActions(actions, commandEnvironment).Execute();
                            break;
                    }
                }
            }
            finally
            {
                Microsoft.Test.Input.Mouse.Reset();
                Microsoft.Test.Input.Keyboard.Reset();
            }

            return Response.CreateSuccessResponse();
        }
    }
}
