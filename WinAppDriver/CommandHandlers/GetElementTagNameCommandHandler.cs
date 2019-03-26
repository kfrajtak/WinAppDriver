using System.Collections.Generic;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the get element text command.
    /// </summary>
    internal class GetElementTagNameCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var value = (ControlType)automationElement.GetAutomationElementPropertyValue("ControlType");
            return Response.CreateSuccessResponse(value.ProgrammaticName);
        }
    }
}
