using WinAppDriver.Server;
using System;

namespace WinAppDriver.Exceptions
{
    public class StaleElementReferenceException : Exception, IRemoteException
    {
        public StaleElementReferenceException(string message = null) : base(message) { }

        public Response GetResponse()
        {
            return Response.CreateErrorResponse(WebDriverStatusCode.ObsoleteElement, "An element command failed because the referenced element is no longer attached to the DOM.");
        }
    }
}
