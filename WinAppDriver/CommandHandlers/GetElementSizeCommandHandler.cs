using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the get element size command.
    /// </summary>
    internal class GetElementSizeCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            var rect = automationElement.Current.BoundingRectangle;
            return Response.CreateSuccessResponse(new { width = (int)rect.Width, height = (int)rect.Height });
        }
    }
}
