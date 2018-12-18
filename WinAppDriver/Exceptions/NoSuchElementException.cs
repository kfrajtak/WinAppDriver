using WinAppDriver.Server;
using System;

namespace WinAppDriver.Exceptions
{
    public class NoSuchElementException : Exception
    {
        public NoSuchElementException(string message = null) : base(message) { }

        public Response GetResponse()
        {
            return Response.CreateErrorResponse(WebDriverStatusCode.NoSuchElement, Message);
        }
    }
}
