using WinAppDriver.Server;

namespace WinAppDriver.Exceptions
{
    public interface IRemoteException
    {
        Response GetResponse();
    }
}
