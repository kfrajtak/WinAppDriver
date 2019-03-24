using System.Collections.Generic;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class IsElementEnabledCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var isEnabled = automationElement.GetAutomationElementPropertyValue("IsEnabled");
            return Response.CreateSuccessResponse(isEnabled);
        }
    }
}
