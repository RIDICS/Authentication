using System.Net.Http;
using System.Threading.Tasks;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.HttpClient.Configuration;

namespace Ridics.Authentication.HttpClient.Client.Auth
{
    public class ContactApiClient : BaseApiClient
    {
        public ContactApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.ContactBasePath;

        public async Task<bool> ChangeContactAsync(ChangeContactContract contract)
        {
            var fullPath = $"{BasePath}changeContact";
            return await m_authorizationServiceHttpClient.SendRequestAsync<bool>(HttpMethod.Post, fullPath, contract);
        }

        public async Task<bool> ConfirmContactAsync(ConfirmContactContract contract)
        {
            var fullPath = $"{BasePath}confirmContact";
            return await m_authorizationServiceHttpClient.SendRequestAsync<bool>(HttpMethod.Post, fullPath, contract);
        }

        public async Task<bool> ResendCodeAsync(ContactContract contract)
        {
            var fullPath = $"{BasePath}resendcode";
            return await m_authorizationServiceHttpClient.SendRequestAsync<bool>(HttpMethod.Post, fullPath, contract);
        }
    }
}