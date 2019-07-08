using System.Threading.Tasks;
using Ridics.Authentication.HttpClient.Contract;
using Ridics.Authentication.HttpClient.Configuration;

namespace Ridics.Authentication.HttpClient.Client.Auth
{
    public class FileResourceApiClient : BaseApiClient
    {
        public FileResourceApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.FileResourceBasePath;

        public async Task<FileResourceStreamContract> GetAsync(int id)
        {
            return await m_authorizationServiceHttpClient
                .GetItemStreamAsync<FileResourceStreamContract, FileResourceStreamHydrator<FileResourceStreamContract>>(BasePath, id);
        }
    }
}