using WinAppDriver.Server;

namespace WinAppDriver
{
    public static class CommandEnvironmentExtensions
    {
        public static Strategies.SendKeys.SendKeyStrategyType GetSendKeyStrategyType(this CommandEnvironment commandEnvironment)
        {
            var sendKeyStrategy = commandEnvironment.GetDesiredCapabilityValue("sendKeyStrategy") ?? "oneByOne";
            switch (sendKeyStrategy.ToString().ToLowerInvariant())
            {
                case "grouped":
                    return Strategies.SendKeys.SendKeyStrategyType.Grouped;
                case "setvalue":
                    return Strategies.SendKeys.SendKeyStrategyType.SetValue;
            }
            return Strategies.SendKeys.SendKeyStrategyType.OneByOne;
        }

        public static Strategies.SendKeys.ISendKeyStrategy GetSendKeyStrategy(this CommandEnvironment commandEnvironment)
        {
            var sendKeyStrategy = commandEnvironment.GetSendKeyStrategyType();
            switch (sendKeyStrategy)
            {
                case Strategies.SendKeys.SendKeyStrategyType.Grouped:
                    return new Strategies.SendKeys.Grouped();
                case Strategies.SendKeys.SendKeyStrategyType.SetValue:
                    return new Strategies.SendKeys.SetValue();
            }
            return new Strategies.SendKeys.OneByOne();
        }
    }
}
