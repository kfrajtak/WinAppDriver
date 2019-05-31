using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    /// <summary>
    /// Provides handling for the set window rect/size command.
    /// </summary>
    internal class SetWindowRectCommandHandler : CommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            if (!parameters.TryGetValue("width", out var width))
            {
                return Response.CreateMissingParametersResponse("width");
            }

            if (!parameters.TryGetValue("height", out var height))
            {
                return Response.CreateMissingParametersResponse("height");
            }

            Resize(environment.Cache.AutomationElement, int.Parse(width.ToString()), int.Parse(height.ToString()));

            return Response.CreateSuccessResponse();
        }

        // see https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndAfter, int x, int y, int width, int height, int flags);

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;

        public static void Resize(AutomationElement window, int width, int height)
        {
            SetWindowPos(window, 0, 0, width, height, SWP_NOMOVE | SWP_NOZORDER);
        }

        public static void Move(AutomationElement window, int x, int y)
        {
            SetWindowPos(window, x, y, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
        }

        public static void Move(AutomationElement window, int x, int y, int width, int height)
        {
            SetWindowPos(window, x, y, width, height, SWP_NOZORDER);
        }

        private static void SetWindowPos(AutomationElement window, int x, int y, int width, int height, int flags)
        {
            var handle = new IntPtr(window.Current.NativeWindowHandle);
            SetWindowPos(handle, IntPtr.Zero, x, y, width, height, flags);
        }
    }
}
