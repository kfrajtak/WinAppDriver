using WinAppDriver.Server;
using System;

namespace WinAppDriver.Exceptions
{
    public class InvalidSelectorException : Exception
    {
        public InvalidSelectorException(string message = null) : base(message) { }

        public Response GetResponse()
        {
            return Response.CreateErrorResponse(WebDriverStatusCode.InvalidSelector /*32*/, Message);
        }
    }
}
