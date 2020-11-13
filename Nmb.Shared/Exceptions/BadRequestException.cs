using System;

namespace Nmb.Shared.Exceptions
{
    public class BadRequestException: Exception
    {
        public int StatusCode { get; set; }

        public BadRequestException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
