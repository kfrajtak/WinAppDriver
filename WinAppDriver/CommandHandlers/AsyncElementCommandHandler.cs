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

        protected abstract Task<Response> GetResponseAsync(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters);
    }
}
