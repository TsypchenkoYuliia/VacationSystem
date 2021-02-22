using System;


namespace WebApi.Exceptions
{
    public class AuthorizeException:Exception
    {
        public int StatusCode { get; set; }
        public AuthorizeException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
