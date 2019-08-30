using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Windows.Automation;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides the base class for handling for the commands.
    /// </summary>
    internal abstract class ElementCommandHandler : CommandHandler
    {
        protected string _parameterName = "ID";

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            if (!parameters.TryGetValue(_parameterName, out object id))
            {
                return Response.CreateMissingParametersResponse(_parameterName);
            }

            var element = environment.Cache.GetElement(id);
            if (element != null)
            {
                return TryGetResponse(element, environment, parameters, cancellationToken);
            }

            throw new Exceptions.NoSuchElementException();
        }

        public Response TryGetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                return GetResponse(automationElement, environment, parameters, cancellationToken);
            }
            catch (Exception ex)
            {
                return FromException(automationElement, ex, environment, cancellationToken);
            }
        }

        protected abstract Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken);

        private Response FromException(AutomationElement element, Exception exception, CommandEnvironment environment, CancellationToken cancellationToken)
        {
            if (exception is AggregateException aggregateException)
            {
                return FromException(element, aggregateException.InnerException, environment, cancellationToken);
            }

            if (exception is ElementNotEnabledException enee)
            {
                // maybe the interaction is not possible, because there is a modal window blocking the UI
                var parentWindow = element.GetTopLevelWindow();
                if (parentWindow.IsBlockedByModalWindow())
                {
                    var modalWindow = environment.GetModalWindow(cancellationToken);

                    return Response.CreateErrorResponse(
                        WebDriverStatusCode.UnexpectedAlertOpen,
                        "Interaction with the element is not possible, because there is a modal window blocking the UI.",
                        payload: modalWindow?.GetWindowCaption(),
                        error: "Invalid element state",
                        sessionId: environment.SessionId);
                }

                return Response.CreateErrorResponse(WebDriverStatusCode.InvalidElementState, enee.Message);
            }

            ExceptionDispatchInfo.Capture(exception).Throw();

            return null;
        }
    }
}
