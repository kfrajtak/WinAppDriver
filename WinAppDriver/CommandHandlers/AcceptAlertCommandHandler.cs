using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using WinAppDriver.Extensions;
using WinAppDriver.XPath.Iterators;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class AcceptAlertCommandHandler : CommandHandler
    {
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var modalWindow = environment.GetModalWindow();
            if (modalWindow == null)
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.NoAlertPresent, string.Empty);
            }

            var buttons = new DescendantIterator(modalWindow, false, environment.GetCancellationToken())
                .Cast<AutomationElement>()
                .Where(el => el.Current.ControlType == ControlType.Button)
                .ToList();

            // button accepting the dialog is identified by its caption (Ok or Yes)
            // TODO how about other languages?
            var acceptButton = buttons.FirstOrDefault(b =>
            {
                var text = b.GetText();
                return text.Equals("Ok", System.StringComparison.CurrentCultureIgnoreCase) ||
                    text.Equals("Yes", System.StringComparison.CurrentCultureIgnoreCase);
            });

            if (acceptButton != null)
            {
                return new ClickElementCommandHandler().GetResponse(acceptButton, environment);
            }

            return Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError,
                "Alert cannot be accepted, no button with caption 'Yes' or 'Ok' found. You have to accept the alert yourself.",
                "Alert cannot be accepted",
                sessionId: environment.SessionId);
        }
    }
}
