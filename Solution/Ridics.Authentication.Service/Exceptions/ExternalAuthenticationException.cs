using System;

namespace Ridics.Authentication.Service.Exceptions
{
    public class ExternalAuthenticationException : Exception
    {
        public ExternalAuthenticationException()
        {
        }

        public ExternalAuthenticationException(string message) : base(message)
        {
        }

        public ExternalAuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}