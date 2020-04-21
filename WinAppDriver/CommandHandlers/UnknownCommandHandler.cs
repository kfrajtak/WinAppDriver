using System.Collections.Generic;
using System.Threading;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class UnknownCommandHandler : CommandHandler
    {
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            var method = parameters["method"];
            var uri = parameters["uri"];
            return Response.CreateErrorResponse(WebDriverStatusCode.UnknownCommand, $"{method} request {uri} cannot be handled (not supported).");
        }
    }
}
