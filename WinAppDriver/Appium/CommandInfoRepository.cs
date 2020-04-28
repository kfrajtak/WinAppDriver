using OpenQA.Selenium.Remote;

namespace WinAppDriver.Server.Appium
{
    public class CommandInfoRepository
    {
        public static void AddAppiumCommands(W3CWireProtocolCommandInfoRepository commandInfoRepository)
        {
            commandInfoRepository.TryAddCommand(
                DriverCommand.CloseApp, 
                new CommandInfo("POST", @"/session/{sessionId}/appium/app/close"));
        }
    }
}
