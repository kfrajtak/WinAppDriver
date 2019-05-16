using WinAppDriver.Server;
using System;

namespace WinAppDriver.Exceptions
{
    public class ElementNotDisplayedException : Exception, IRemoteException
    {
        public ElementNotDisplayedException(string message = null) : base(message) { }

        public Response GetResponse()
        {
            return Response.CreateErrorResponse(WebDriverStatusCode.ElementNotDisplayed, Message);
        }
    }
}
