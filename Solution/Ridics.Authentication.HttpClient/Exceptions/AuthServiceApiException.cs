using Ridics.Core.HttpClient.Exceptions;

namespace Ridics.Authentication.HttpClient.Exceptions
{
    public class AuthServiceApiException : ServiceApiException
    {
        public AuthServiceApiException(string message) : base(message)
        {
        }
    }
}