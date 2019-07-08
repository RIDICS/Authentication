using Ridics.Authentication.HttpClient.Exceptions;

namespace Ridics.Authentication.HttpClient.Client
{
    public interface IAuthorizationServiceClientLocalization
    {
        void LocalizeApiException(AuthServiceApiException ex);
        string GetCurrentCulture();
    }
}