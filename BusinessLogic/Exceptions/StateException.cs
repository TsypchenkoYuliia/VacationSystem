using System;


namespace BusinessLogic.Exceptions
{
    public class StateException:Exception
    {
        public int StatusCode { get; set; }
        public StateException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
