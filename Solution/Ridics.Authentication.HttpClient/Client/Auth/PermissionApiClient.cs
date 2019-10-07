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

        public Task<ListContract<PermissionContract>> GetPermissionListAsync(int start, int count, string search = null, bool fetchPermissions = false)
        {
            var parameters = m_authorizationServiceHttpClient.CreateQueryCollection();
            parameters.Add("fetchPermissions", fetchPermissions.ToString());

            return m_authorizationServiceHttpClient.GetListAsync<PermissionContract>(start, count, search, parameters);
        }

        public Task<PermissionContract> GetItemAsync(int id, bool fetchPermissions = false)
        {
            var parameters = m_authorizationServiceHttpClient.CreateQueryCollection();
            parameters.Add("fetchPermissions", fetchPermissions.ToString());

            return m_authorizationServiceHttpClient.GetItemAsync<PermissionContract>(id, parameters);
        }

        public Task<HttpResponseMessage> Create(PermissionContractBase permissionContract)
        {
            return m_authorizationServiceHttpClient.CreateItemAsync(permissionContract);
        }

        public Task<HttpResponseMessage> Edit(int id, PermissionContractBase permissionContract)
        {
            return m_authorizationServiceHttpClient.EditItemAsync(id, permissionContract);
        }

        public Task<HttpResponseMessage> Delete(int id)
        {
            return m_authorizationServiceHttpClient.DeleteItemAsync<PermissionContract>(id);
        }

        public async Task AssignRolesToPermissionAsync(int id, IEnumerable<int> selectedRoles)
        {
            var fullPath = $"{BasePath}{id}/Roles";
            await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, selectedRoles);
        }

        public async Task<IList<PermissionContractBase>> GetAllPermissionsAsync(string search = null)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();
            if (search != null)
            {
                query.Add("search", search);
            }

            var fullPath = $"{BasePath}AllPermissions";
            if (query.Count > 0)
            {
                fullPath = $"{fullPath}?{query}";
            }

            return await m_authorizationServiceHttpClient.SendRequestAsync<IList<PermissionContractBase>>(HttpMethod.Get, fullPath);
        }

        public async Task<bool> CheckUserHasPermissionAsync(int userId, string permissionName)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();
            query.Add("userId", userId.ToString());
            query.Add("permissionName", permissionName);

            var fullPath = $"{BasePath}check?{query}";

            return await m_authorizationServiceHttpClient.SendRequestAsync<bool>(HttpMethod.Get, fullPath);
        }

        public Task EnsurePermissionsExistAsync(EnsurePermissionsContract data)
        {
            var fullPath = $"{BasePath}ensure";
            return m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Put, fullPath, data);
        }
    }
}