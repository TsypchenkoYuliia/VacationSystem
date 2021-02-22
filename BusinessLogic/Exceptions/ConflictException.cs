using System;

namespace BusinessLogic.Exceptions
{
    public class ConflictException : Exception
    {
        public int StatusCode { get; set; }
        public ConflictException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
