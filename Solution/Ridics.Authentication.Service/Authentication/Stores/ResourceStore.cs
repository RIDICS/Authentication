using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Ridics.Authentication.Core.Managers;

namespace Ridics.Authentication.Service.Authentication.Stores
{
    public class ResourceStore : IResourceStore
    {
        private readonly ApiResourceManager m_apiResourceManager;
        private readonly IdentityResourceManager m_identityResourceManager;

        public ResourceStore(ApiResourceManager apiResourceManager, IdentityResourceManager identityResourceManager)
        {
            m_apiResourceManager = apiResourceManager;
            m_identityResourceManager = identityResourceManager;
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }

            var identityResources = GetIdentityResources();

            return Task.FromResult(identityResources?.Where(x => scopeNames.Contains(x.Name)));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            if (scopeNames == null)
            {
                throw new ArgumentNullException(nameof(scopeNames));
            }

            var apiResources = GetApiResources();

            var result = apiResources.Where(x => x.Scopes.FirstOrDefault(y => scopeNames.Contains(y.Name)) != null);

            return Task.FromResult(result);
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return Task.FromResult(FindApiResource(name));
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            return Task.FromResult(GetAllResources());
        }

        private Resources GetAllResources()
        {
            var apiResources = GetApiResources();
            var identityResources = GetIdentityResources();

            return new Resources(identityResources, apiResources);
        }

        private IEnumerable<ApiResource> GetApiResources()
        {
            var apiResourceResult = m_apiResourceManager.GetAllApiResources();
            if (apiResourceResult.HasError)
            {
                return null;
            }

            var apiResources = Mapper.Map<List<ApiResource>>(apiResourceResult.Result);

            return apiResources;
        }

        private IEnumerable<IdentityResource> GetIdentityResources()
        {
            var identityResourceResult = m_identityResourceManager.GetAllIdentityResources();
            if (identityResourceResult.HasError)
            {
                return null;
            }

            var identityResources = Mapper.Map<List<IdentityResource>>(identityResourceResult.Result);

            return identityResources;
        }

        private ApiResource FindApiResource(string name)
        {
            var apiResourceResult = m_apiResourceManager.FindApiResourceByName(name);

            if (apiResourceResult.HasError)
            {
                return null;
            }

            var apiResource = Mapper.Map<ApiResource>(apiResourceResult.Result);

            return apiResource;
        }
    }
}