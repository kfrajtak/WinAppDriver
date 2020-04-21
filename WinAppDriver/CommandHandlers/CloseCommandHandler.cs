using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the close command.
    /// </summary>
    internal class CloseCommandHandler : CommandHandler
    {
        /// <summary>
        /// Closes the current window
        /// </summary>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            var hwnd = environment.WindowHandle;
            if (hwnd == System.IntPtr.Zero)
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.NoSuchWindow, $"Currently selected window has been closed");
            }

            var window = environment.Cache.AutomationElement;
            if (window.TryGetCurrentPattern(WindowPattern.Pattern, out object pattern) && pattern is WindowPattern windowPattern)
            {
                environment.CloseWindow(hwnd);
                windowPattern.Close();
                return Response.CreateSuccessResponse();
            }

            return Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError, "Cannot close current window.");
        }
    }
}
