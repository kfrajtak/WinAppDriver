using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides the base class for handling for the commands.
    /// </summary>
    internal abstract class ElementCommandHandler : CommandHandler
    {
        private TimeSpan atomExecutionTimeout = TimeSpan.FromMilliseconds(-1);

        protected string _parameterName = "ID";

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            if (!parameters.TryGetValue(_parameterName, out object id))
            {
                return Response.CreateMissingParametersResponse(_parameterName);
            }

            var element = environment.Cache.GetElement(id);
            if (element != null)
            {
                return GetResponse(element, environment, parameters);

            }

            throw new Exceptions.NoSuchElementException();
        }

        protected abstract Response GetResponse(AutomationElement automationElement, CommandEnvironment environment, Dictionary<string, object> parameters);
    }
}
