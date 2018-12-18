using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var pattern in automationElement.GetSupportedPatterns())
            {
                System.Diagnostics.Debug.WriteLine("pattern: " + pattern.ProgrammaticName);
            }

            foreach (var prop in automationElement.GetSupportedProperties().OrderBy(p => p.ProgrammaticName))
            {
                System.Diagnostics.Debug.WriteLine("property: " + prop.ProgrammaticName);
            }

            if (!automationElement.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var selectionItemPattern))
            {
                throw new NotSupportedException();
            }

            return Response.CreateSuccessResponse(((SelectionItemPattern)selectionItemPattern).Current.IsSelected);
        }
    }
}
