using System;

namespace WebApi.Exceptions
{
    public class CreateException:Exception
    {
        public int StatusCode { get; set; }
        public CreateException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
