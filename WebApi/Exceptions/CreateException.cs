using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Exceptions
{
    public class CreateException : Exception
    {
        public int StatusCode { get; set; }
        public CreateException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
