using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the is element displayed command.
    /// </summary>
    internal class IsElementDisplayedCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            return Response.CreateSuccessResponse(!automationElement.Current.IsOffscreen);
        }
    }
}
