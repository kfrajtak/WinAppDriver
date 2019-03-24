using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the get element rect command.
    /// </summary>
    internal class GetElementRectCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var rect = automationElement.Current.BoundingRectangle;
            return Response.CreateSuccessResponse(new { x = (int)rect.Left, y = (int)rect.Top, width = (int)rect.Width, height = (int)rect.Height });
        }
    }
}
