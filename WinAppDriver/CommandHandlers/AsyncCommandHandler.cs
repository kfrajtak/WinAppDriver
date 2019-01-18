using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides the base class for handling for the commands.
    /// </summary>
    internal abstract class AsyncCommandHandler : CommandHandler
    {
        private TimeSpan atomExecutionTimeout = TimeSpan.FromMilliseconds(-1);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public abstract Task<Response> ExecuteAsync(CommandEnvironment environment, Dictionary<string, object> parameters);
    }
}
