using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

            var release = new Stack<Microsoft.Test.Input.Key>();

            var actionsToExecute = new List<Action>();

            SetForegroundWindow(commandEnvironment.WindowHandle);

            System.Windows.Automation.AutomationElement.FromHandle(commandEnvironment.WindowHandle).SetFocus();

            Microsoft.Test.Input.Mouse.Reset();
            Microsoft.Test.Input.Keyboard.Reset();

            try
            {
                foreach (var block in (JArray)o)
                {
                    var actions = (JArray)block["actions"];
                    var type = block["type"].Value<string>();

                    switch (type)
                    {
                        case "pointer":
                            new MouseActions(actions, commandEnvironment).Execute();
                            break;
                        case "key":
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
