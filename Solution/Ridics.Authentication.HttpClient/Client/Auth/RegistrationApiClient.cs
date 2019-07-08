using System.Net.Http;
using System.Threading.Tasks;
using Ridics.Authentication.DataContracts.User;
using Ridics.Authentication.HttpClient.Configuration;

namespace Ridics.Authentication.HttpClient.Client.Auth
{
    public class RegistrationApiClient : BaseApiClient
    {
        public RegistrationApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.RegistrationBasePath;

        public async Task<UserContract> CreateUserAsync(CreateUserContract contract)
        {
            var fullPath = $"{BasePath}create";
            return await m_authorizationServiceHttpClient.SendRequestAsync<UserContract>(HttpMethod.Post, fullPath, contract);
        }

        public async Task<VerifiedUserCreatedContract> CreateVerifiedUserAsync(CreateVerifiedUserContract contract)
        {
            var fullPath = $"{BasePath}createVerified";
            return await m_authorizationServiceHttpClient.SendRequestAsync<VerifiedUserCreatedContract>(HttpMethod.Post, fullPath,
                contract);
        }

        public async Task<UserContract> VerifyUserAsync(UserContractBase contract, string registrationCode)
        {
            var fullPath = $"{BasePath}verifyUser?code={registrationCode}";
            return await m_authorizationServiceHttpClient.SendRequestAsync<UserContract>(HttpMethod.Post, fullPath, contract);
        }
        
        public async Task<UserContract> SearchUserAsync(string registrationCode)
        {
            var fullPath = $"{BasePath}search?code={registrationCode}";
            return await m_authorizationServiceHttpClient.SendRequestAsync<UserContract>(HttpMethod.Get, fullPath);
        }

        public async Task<UserInfoContract> SearchUserInfoAsync(string registrationCode)
        {
            var fullPath = $"{BasePath}searchinfo?code={registrationCode}";
            return await m_authorizationServiceHttpClient.SendRequestAsync<UserInfoContract>(HttpMethod.Get, fullPath);
        }
    }
}