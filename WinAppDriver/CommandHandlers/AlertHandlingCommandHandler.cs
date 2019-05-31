using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using WinAppDriver.Extensions;
using WinAppDriver.XPath.Iterators;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class AcceptAlertCommandHandler : AlertHandlingCommandHandler
    {
        public AcceptAlertCommandHandler() : base("accept", "acceptAlertButtonCaptions", "Ok;Yes")
        {
        }
    }

    internal class DismissAlertCommandHandler : AlertHandlingCommandHandler
    {
        public DismissAlertCommandHandler() : base("dismiss", "dismissAlertButtonCaptions", "Cancel;No")
        {
        }
    }

    internal class AlertHandlingCommandHandler : CommandHandler
    {
        private readonly string _capabilityName;
        private readonly string _defaultCaptions;
        private readonly string _verb;

        public AlertHandlingCommandHandler(string verb, string capabilityName, string defaultCaptions)
        {
            _verb = verb;
            _capabilityName = capabilityName;
            _defaultCaptions = defaultCaptions;
        }

        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            var modalWindow = environment.GetModalWindow(cancellationToken);
            if (modalWindow == null)
            {
                return Response.CreateErrorResponse(WebDriverStatusCode.NoAlertPresent, string.Empty);
            }

            var buttons = new DescendantIterator(modalWindow, false, cancellationToken)
                .Cast<AutomationElement>()
                .Where(el => el.Current.ControlType == ControlType.Button)
                .ToList();

            var captions = (environment.GetDesiredCapabilityValue(_capabilityName) + ";" + _defaultCaptions)
                .ToString()
                .Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);

            // button accepting/dismissing the dialog is identified by its caption
            var button = buttons.FirstOrDefault(b =>
            {
                var text = b.GetText();
                return captions.Any(c => text.Equals(c, System.StringComparison.CurrentCultureIgnoreCase));
            });

            if (button != null)
            {
                return new ClickElementCommandHandler().GetResponse(button, environment);
            }

            return Response.CreateErrorResponse(WebDriverStatusCode.UnhandledError,
                $"Modal dialog cannot be {_verb}ed, button with one of default captions ({string.Join(", ", _defaultCaptions.Split(';'))}) was not found. You have to {_verb} the modal dialog yourself or set the driver capability '{_capabilityName}' with semicolon delimited list of button captions for the {_verb} command to succeed.",
                $"Modal dialog cannot be {_verb}ed",
                sessionId: environment.SessionId);
        }
    }
}
