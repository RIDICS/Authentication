using System.Threading.Tasks;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.HttpClient.Configuration;

namespace Ridics.Authentication.HttpClient.Client.Auth
{
    public class ExternalIdentityProviderApiClient : BaseApiClient
    {
        public ExternalIdentityProviderApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.ExternalLoginProviderBasePath;

        public async Task<ListContract<ExternalLoginProviderContract>> ListExternalIdentityProviderAsync(
            int start = 0, int count = DefaultListCount
        )
        {
            return await m_authorizationServiceHttpClient.GetListAsync<ExternalLoginProviderContract>(start, count);
        }
    }
}