using System.Collections.Generic;
using System.Threading;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class GetTitleCommandHandler : ElementCommandHandler
    {
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, CancellationToken cancellationToken)
        {
            if (parameters.TryGetValue(_parameterName, out object id))
            {
                return base.Execute(environment, parameters, cancellationToken);
            }

            return TryGetResponse(environment.RootElement, environment, parameters, cancellationToken);
        }

        protected override Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            return Response.CreateSuccessResponse(automationElement.GetWindowCaption());
        }
    }
}
