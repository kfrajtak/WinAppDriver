using System.Collections.Generic;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class IsElementEnabledCommandHandler : ElementCommandHandler
    {
        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            // walk the tree up to be sure all element ancestors are enabled
            do
            {
                var isEnabled = automationElement.GetAutomationElementPropertyValue("IsEnabled");
                if (!(bool)isEnabled)
                {
                    return Response.CreateSuccessResponse(isEnabled);
                }

                automationElement = TreeWalker.RawViewWalker.GetParent(automationElement);
            }
            while (automationElement != null);

            return Response.CreateSuccessResponse(true);
        }
    }
}
