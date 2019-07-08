using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Ridics.Authentication.DataContracts.User;
using Ridics.Authentication.Service.Authorization;

namespace Ridics.Authentication.Service.Helpers
{
    public class UserHelper
    {
        private readonly TwoFactorProvidersProvider m_twoFactorProvidersProvider;
        private readonly IAuthorizationService m_authorizationService;
        private readonly IHttpContextAccessor m_httpContextAccessor;

        public UserHelper(TwoFactorProvidersProvider twoFactorProvidersProvider, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            m_twoFactorProvidersProvider = twoFactorProvidersProvider;
            m_authorizationService = authorizationService;
            m_httpContextAccessor = httpContextAccessor;
        }

        public async Task FillValidTwoFactorProvidersAsync(UserContractBase userContract)
        {
            var providers = await m_twoFactorProvidersProvider.GetValidTwoFactorProvidersForUserAsync(userContract.Id);

            userContract.ValidTwoFactorProviders = providers;
        }

        public async Task FillValidTwoFactorProvidersAsync(IEnumerable<UserContract> userContracts)
        {
            foreach (var userContract in userContracts)
            {
                await FillValidTwoFactorProvidersAsync(userContract);
            }
        }

        public async Task<bool> IsUserAuthorizedAsync(int userId, bool needPermissionToEditSelf, bool canOtherUserEdit, string permissionRequiredToEditSelf = null, string permissionRequiredToEditOther = null)
        {
            var user = m_httpContextAccessor.HttpContext.User;

            var authorized = await m_authorizationService.ApiAuthorizationForUserEditAsync(user, userId, needPermissionToEditSelf, canOtherUserEdit, permissionRequiredToEditSelf, permissionRequiredToEditOther);

            return authorized;
        }
    }
}