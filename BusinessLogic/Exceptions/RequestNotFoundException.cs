using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Exceptions
{
    public class RequestNotFoundException : Exception
    {
        public int StatusCode { get; set; }
        public RequestNotFoundException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
