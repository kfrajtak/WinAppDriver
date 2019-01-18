using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the is element selected command.
    /// </summary>
    internal class IsElementSelectedCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            if (!automationElement.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var selectionItemPattern))
            {
                throw new NotSupportedException();
            }

            return Response.CreateSuccessResponse(((SelectionItemPattern)selectionItemPattern).Current.IsSelected);
        }
    }
}
