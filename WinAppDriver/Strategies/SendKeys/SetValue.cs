using System.Collections.Generic;
using System.Threading;
using WinAppDriver.Extensions;
using WinAppDriver.Server;

namespace WinAppDriver.Strategies.SendKeys
{
    public class SetValue : ISendKeyStrategy
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public Response Execute(CommandEnvironment commandEnvironment, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            if (!parameters.TryGetValue("text", out var text))
            {
                return Response.CreateMissingParametersResponse("text");
            }

            var id = parameters["ID"];
            var automationElement = commandEnvironment.Cache.GetElement(id);
            automationElement.SetText(text?.ToString() ?? "");

            Logger.Info($"Text {text} set to element {id}.");

            return Response.CreateSuccessResponse();
        }
    }
}
