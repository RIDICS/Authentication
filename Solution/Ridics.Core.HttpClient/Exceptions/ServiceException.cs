using System;

namespace Ridics.Core.HttpClient.Exceptions
{
    public class ServiceException : Exception
    {
        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int StatusCode { get; set; }

        public string Content { get; set; }

        public string ContentType { get; set; }
    }
}
