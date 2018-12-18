using WinAppDriver.Server;
using System;

namespace WinAppDriver.Exceptions
{
    public class ObsoleteElementException : Exception
    {
        public ObsoleteElementException(string message = null) : base(message) { }

        public Response GetResponse()
        {
            return Response.CreateErrorResponse(WebDriverStatusCode.ObsoleteElement, Message);
        }
    }
}
