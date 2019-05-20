using WinAppDriver.Server;
using WinAppDriver.Server.CommandHandlers;
using System;
using System.Collections.Generic;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class GetAlertTextCommandHandler : CommandHandler
    {
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var modalWindow = environment.GetModalWindow();
            if (modalWindow == null)
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.NoAlertPresent, string.Empty);
            }

            return Response.CreateSuccessResponse("Blabla");
        }
    }
}
