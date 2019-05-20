using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using Newtonsoft.Json;
using WinAppDriver.Exceptions;
using WinAppDriver.Extensions;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides the base class for asynchronous handling for the commands.
    /// </summary>
    internal abstract class AsyncElementCommandHandler : AsyncCommandHandler
    {
        protected const string _parameterName = "ID";

        public override Task<Response> ExecuteAsync(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            if (!parameters.TryGetValue(_parameterName, out object id))
            {
                throw new MissingParameterException(_parameterName);
            }

            var element = environment.Cache.GetElement(id);
            if (element != null)
            {
                return GetResponseAsync(element, environment, parameters);
            }

            throw new NoSuchElementException();
        }

        private Response FromException(AutomationElement element, Exception exception)
        {
            if (exception is AggregateException aggregateException)
            {
                return FromException(element, aggregateException.InnerException);
            }

            if (exception is ElementNotEnabledException enee)
            {
                // maybe the interaction is not possible, because there is a modal window blocking the UI
                var parentWindow = element.GetTopLevelWindow();
                if (parentWindow.IsBlockedByModalWindow())
                {
                    return Response.CreateErrorResponse(WebDriverStatusCode.UnexpectedAlertOpen, "Interaction with the element is not possible, because there is a modal window blocking the UI.");
                }

                return Response.CreateErrorResponse(WebDriverStatusCode.InvalidElementState, enee.Message);
            }

            throw exception;
        }

        //protected abstract Task<Response> GetResponseAsync(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters);
        protected virtual Task<Response> GetResponseAsync(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            return new Task<Response>(() =>
            {
                try
                {
                    return GetResponse(automationElement, environment, parameters);
                }
                catch (Exception ex)
                {
                    return FromException(automationElement, ex);
                }
            });
        }

        protected abstract Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters);
    }
}
