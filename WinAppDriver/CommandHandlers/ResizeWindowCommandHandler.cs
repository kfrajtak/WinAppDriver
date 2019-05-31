using System;
using System.Collections.Generic;
using System.Windows.Automation;

namespace WinAppDriver.Server.CommandHandlers
{
    internal class ResizeWindowCommandHandler : CommandHandler
    {
        private readonly WindowVisualState _requiredVisualState;

        public ResizeWindowCommandHandler(WindowVisualState requiredVisualState)
        {
            _requiredVisualState = requiredVisualState;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters, System.Threading.CancellationToken cancellationToken)
        {
            var windowPattern = GetWindowPattern(environment.Cache.AutomationElement);
            var windowInteractionState = windowPattern.Current.WindowInteractionState;
            if (windowInteractionState != WindowInteractionState.ReadyForUserInteraction)
            {
                throw new Exception("Invalid window interaction state - " + windowInteractionState);
            }

            switch (_requiredVisualState)
            {
                case WindowVisualState.Maximized:
                    // Confirm that the element can be maximized
                    if (windowPattern.Current.CanMaximize && !windowPattern.Current.IsModal)
                    {
                        windowPattern.SetWindowVisualState(WindowVisualState.Maximized);
                    }
                    break;
                case WindowVisualState.Minimized:
                    // Confirm that the element can be minimized
                    if ((windowPattern.Current.CanMinimize) &&
                        !(windowPattern.Current.IsModal))
                    {
                        windowPattern.SetWindowVisualState(WindowVisualState.Minimized);
                        // TODO: additional processing
                    }
                    break;
                case WindowVisualState.Normal:
                    windowPattern.SetWindowVisualState(WindowVisualState.Normal);
                    break;
                default:
                    windowPattern.SetWindowVisualState(WindowVisualState.Normal);
                    // TODO: additional processing
                    break;
            }

            return Response.CreateSuccessResponse();
        }

        ///--------------------------------------------------------------------
        /// <summary>
        /// Obtains a WindowPattern control pattern from an automation element.
        /// </summary>
        /// <param name="targetControl">
        /// The automation element of interest.
        /// </param>
        /// <returns>
        /// A WindowPattern object.
        /// </returns>
        ///--------------------------------------------------------------------
        private WindowPattern GetWindowPattern(AutomationElement targetControl)
        {
            if (!targetControl.TryGetCurrentPattern(WindowPattern.Pattern, out var pattern))
            {
                throw new Exception("Cannot get WindowPattern.Pattern for element.");
            }
            return pattern as WindowPattern;
        }
    }
}
