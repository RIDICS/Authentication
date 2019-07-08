using Ridics.Authentication.HttpClient.Configuration;

namespace Ridics.Authentication.HttpClient.Client.Auth
{
    public abstract class BaseApiClient
    {
        protected const int DefaultListCount = 20;

        protected readonly AuthorizationServiceHttpClient m_authorizationServiceHttpClient;

        protected readonly AuthServiceControllerBasePathsConfiguration m_basePathsConfiguration;

        protected BaseApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        )
        {
            m_authorizationServiceHttpClient = authorizationServiceHttpClient;
            m_basePathsConfiguration = basePathsConfiguration;
        }

        protected abstract string BasePath { get; }

        public AuthorizationServiceHttpClient HttpClient => m_authorizationServiceHttpClient;
    }
}