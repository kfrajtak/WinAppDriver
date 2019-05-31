using System.Collections.Generic;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class GetTitleCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            return Response.CreateSuccessResponse(automationElement.GetWindowCaption());
        }
    }
}
