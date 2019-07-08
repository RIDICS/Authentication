using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Service.Authentication.Identity.Managers;

namespace Ridics.Authentication.Service.Helpers
{
    public class TwoFactorProvidersProvider
    {
        private readonly IdentityUserManager m_identityUserManager;
        private readonly IdentitySignInManager m_identitySignInManager;

        public TwoFactorProvidersProvider(IdentityUserManager identityUserManager, IdentitySignInManager identitySignInManager)
        {
            m_identityUserManager = identityUserManager;
            m_identitySignInManager = identitySignInManager;
        }

        /// <summary>
        /// Get all valid two factor providers for specified user. If there is Default provider in valid two factor providers, adds specific name of default provider
        /// </summary>
        /// <param name="userId">id of user</param>
        /// <returns>Task with list containing all two factor providers for concrete user</returns>
        public async Task<IList<string>> GetValidTwoFactorProvidersForUserAsync(int userId)
        {
            var appUser = await m_identityUserManager.GetUserByIdAsync(userId);

            if (appUser == null)
            {
                return null;
            }

            var validTwoFactorProviders = await m_identityUserManager.GetValidTwoFactorProvidersAsync(appUser);

            for (var i = 0; i < validTwoFactorProviders.Count; i++)
            {
                if (validTwoFactorProviders[i] == TokenOptions.DefaultProvider)
                {
                    var defaultProvider = m_identitySignInManager.ResolveDefaultTokenProvider();
                    validTwoFactorProviders[i] = $"{TokenOptions.DefaultProvider}-{defaultProvider}";
                    break;
                }
            }

            return validTwoFactorProviders;
        }

        /// <summary>
        /// If <paramref name="twoFactorProvider"/> is Default with specific name of provider, added with <see cref="GetValidTwoFactorProvidersForUserAsync"/>, removes the specific name.
        /// </summary>
        /// <param name="twoFactorProvider">Two factor provider name with possible specific default provider name</param>
        /// <returns>Valid two factor provider</returns>
        public string GetTwoFactorProviderName(string twoFactorProvider)
        {
            if (twoFactorProvider.StartsWith(TokenOptions.DefaultProvider))
            {
                return TokenOptions.DefaultProvider;
            }

            return twoFactorProvider;
        }
    }
}