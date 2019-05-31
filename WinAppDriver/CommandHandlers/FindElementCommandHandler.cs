using System.Collections.Generic;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the find element command.
    /// </summary>
    internal class FindElementCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            return new FindChildElementCommandHandler().TryGetResponse(environment.Cache.AutomationElement, environment, parameters);
        }
    }
}
