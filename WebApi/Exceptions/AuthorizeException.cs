using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
