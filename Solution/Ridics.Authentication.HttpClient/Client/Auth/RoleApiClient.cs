using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.HttpClient.Configuration;

namespace Ridics.Authentication.HttpClient.Client.Auth
{
    public class RoleApiClient : BaseApiClient
    {
        public RoleApiClient(
            AuthorizationServiceHttpClient authorizationServiceHttpClient,
            AuthServiceControllerBasePathsConfiguration basePathsConfiguration
        ) : base(authorizationServiceHttpClient, basePathsConfiguration)
        {
        }

        protected override string BasePath => m_basePathsConfiguration.RoleBasePath;

        public Task<ListContract<RoleContract>> GetRoleListAsync(int start, int count, string search = null, bool fetchPermissions = false)
        {
            var parameters = m_authorizationServiceHttpClient.CreateQueryCollection();
            parameters.Add("fetchPermissions", fetchPermissions.ToString());

            return m_authorizationServiceHttpClient.GetListAsync<RoleContract>(start, count, search, parameters);
        }

        public Task<RoleContract> GetRoleAsync(int id, bool fetchPermissions = false)
        {
            var parameters = m_authorizationServiceHttpClient.CreateQueryCollection();
            parameters.Add("fetchPermissions", fetchPermissions.ToString());

            return m_authorizationServiceHttpClient.GetItemAsync<RoleContract>(id, parameters);
        }

        public Task<HttpResponseMessage> CreateRoleAsync(RoleContractBase roleContract)
        {
            return m_authorizationServiceHttpClient.CreateItemAsync(roleContract);
        }

        public Task<HttpResponseMessage> EditRoleAsync(int id, RoleContractBase roleContract)
        {
            return m_authorizationServiceHttpClient.EditItemAsync(id, roleContract);
        }

        public Task<HttpResponseMessage> DeleteRoleAsync(int id)
        {
            return m_authorizationServiceHttpClient.DeleteItemAsync<RoleContract>(id);
        }

        public async Task AssignPermissionsToRoleAsync(int id, IEnumerable<int> selectedPermissions)
        {
            var fullPath = $"{BasePath}{id}/Permissions";
            await m_authorizationServiceHttpClient.SendRequestAsync(HttpMethod.Post, fullPath, selectedPermissions);
        }

        public async Task<IList<RoleContractBase>> GetAllRolesAsync()
        {
            var fullPath = $"{BasePath}AllRoles";
            return await m_authorizationServiceHttpClient.SendRequestAsync<IList<RoleContractBase>>(HttpMethod.Get, fullPath);
        }

        public async Task<ListContract<UserWithRolesContract>> GetUserListByRoleAsync(int roleId, int? start, int? count, string search)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();

            if (start != null) query.Add("start", start.Value.ToString());
            if (count != null) query.Add("count", count.Value.ToString());
            if (!string.IsNullOrEmpty(search)) query.Add("search", search);

            var path = $"{BasePath}{roleId}/Users?{query}";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<ListContract<UserWithRolesContract>>(HttpMethod.Get, path);

            return response;
        }

        public async Task<RoleContract> GetRoleByName(string name, bool fetchPermissions = false)
        {
            var query = m_authorizationServiceHttpClient.CreateQueryCollection();
            query.Add("name", name);
            query.Add("fetchPermissions", fetchPermissions.ToString());

            var path = $"{BasePath}?{query}";
            var response = await m_authorizationServiceHttpClient.SendRequestAsync<RoleContract>(HttpMethod.Get, path);

            return response;
        }
    }
}