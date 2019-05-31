using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Automation;
using WinAppDriver.Exceptions;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides the base class for asynchronous handling for the commands.
    /// </summary>
    internal abstract class AsyncElementCommandHandler : ElementCommandHandler, IAsyncCommandHandler
    {
        public Task<Response> ExecuteAsync(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            if (!parameters.TryGetValue(_parameterName, out object id))
            {
                throw new MissingParameterException(_parameterName);
            }

            var element = environment.Cache.GetElement(id);
            if (element != null)
            {
                return GetResponseAsync(element, environment, parameters, cancellationToken);
            }

            throw new NoSuchElementException();
        }

        protected virtual Task<Response> GetResponseAsync(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            return new Task<Response>(() =>
            {
                return TryGetResponse(automationElement, environment, parameters, cancellationToken);
            }, cancellationToken);
        }
    }
}
