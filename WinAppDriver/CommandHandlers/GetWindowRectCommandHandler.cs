using System.Collections.Generic;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the get window rect command.
    /// </summary>
    internal class GetWindowRectCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var rect = environment.Cache.AutomationElement.Current.BoundingRectangle;
            return Response.CreateSuccessResponse(new { x = (int)rect.Left, y = (int)rect.Top, width = (int)rect.Width, height = (int)rect.Height });
        }
    }
}
