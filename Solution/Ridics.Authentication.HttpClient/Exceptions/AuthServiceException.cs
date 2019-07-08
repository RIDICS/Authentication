using System;
using Ridics.Core.HttpClient.Exceptions;

namespace Ridics.Authentication.HttpClient.Exceptions
{
    public class AuthServiceException : ServiceException
    {
        public AuthServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}