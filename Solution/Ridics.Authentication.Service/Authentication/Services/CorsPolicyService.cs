using System.Threading.Tasks;
using IdentityServer4.Services;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Helpers;

namespace Ridics.Authentication.Service.Authentication.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly UriManager m_uriManager;

        public CorsPolicyService(UriManager uriManager)
        {
            m_uriManager = uriManager;
        }

        //Check if redirect uri is allowed
        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            var urisResult = m_uriManager.GetAllUris();

            if (urisResult.HasError)
            {
                return Task.FromResult(false); //TODO propagate error
            }

            var result = urisResult.Result.Find(x => x.IsUriOfType(UriModelHelper.CorsOrigin) && x.Value == origin);

            return Task.FromResult(result != null);
        }
    }
}