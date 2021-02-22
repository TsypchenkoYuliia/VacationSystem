using System;

namespace BusinessLogic.Exceptions
{
    public class RequiredArgumentNullException:Exception
    {
        public int StatusCode { get; set; }
        public RequiredArgumentNullException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }        
    }
}
