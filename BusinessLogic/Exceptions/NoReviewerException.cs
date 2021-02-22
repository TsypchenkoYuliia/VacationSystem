using System;


namespace BusinessLogic.Exceptions
{
    public class NoReviewerException : Exception
    {
        public int StatusCode { get; set; }
        public NoReviewerException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
