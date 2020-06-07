using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using WinAppDriver.Input;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the mouse button up command.
    /// </summary>
    internal class MouseButtonUpCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="commandEnvironment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment commandEnvironment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            if (!parameters.TryGetValue("button", out var button))
            {
                button = 0;
            }

            AutomationElement.FromHandle(commandEnvironment.WindowHandle).SetFocus();
            var actions = new JArray{
                new JObject
                {
                    ["type"] = "pointerUp",
                    ["button"] = JToken.FromObject(button)
                }
            };

            new MouseActions(new JArray(actions.ToArray()), commandEnvironment).Execute(out var _);

            return Response.CreateSuccessResponse();
        }
    }
}
