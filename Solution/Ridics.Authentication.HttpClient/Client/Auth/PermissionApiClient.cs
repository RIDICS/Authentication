using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.HttpClient.Configuration;

namespace Ridics.Authentication.HttpClient.Client.Auth
{
    public class PermissionApiClient : BaseApiClient
    {
        public PermissionApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.PermissionBasePath;

        public async Task AssignRolesToPermissionAsync(int id, IEnumerable<int> selectedRoles)
        {
            var fullPath = $"{BasePath}{id}/Roles";
            await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, selectedRoles);
        }

        public async Task<IList<PermissionContract>> GetAllPermissionsAsync()
        {
            var fullPath = $"{BasePath}AllPermissions";
            return await m_authorizationServiceHttpClient.SendRequestAsync<IList<PermissionContract>>(HttpMethod.Get, fullPath);
        }

        public async Task<bool> CheckUserHasPermission(int userId, string permissionName)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();
            query.Add("userId", userId.ToString());
            query.Add("permissionName", permissionName);

            var fullPath = $"{BasePath}check?{query}";

            return await m_authorizationServiceHttpClient.SendRequestAsync<bool>(HttpMethod.Get, fullPath);
        }
    }
}