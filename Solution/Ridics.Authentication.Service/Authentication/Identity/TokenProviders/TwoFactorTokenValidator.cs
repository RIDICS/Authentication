using System;
using System.Threading.Tasks;
using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;

namespace Ridics.Authentication.Service.Authentication.Identity.TokenProviders
{
    public class TwoFactorTokenValidator
    {
        private readonly UserManager m_userManager;
        private readonly TokensExpirationConfiguration m_tokensExpirationConfiguration;

        public TwoFactorTokenValidator(UserManager userManager, TokensExpirationConfiguration tokensExpirationConfiguration)
        {
            m_userManager = userManager;
            m_tokensExpirationConfiguration = tokensExpirationConfiguration;
        }

        public Task<bool> ValidateAsync(string providerName, string token, ApplicationUser user)
        {
            var issuedToken = m_userManager.GetTwoFactorTokenForUser(user.Id, providerName).Result;

            if (issuedToken != null && issuedToken.Token == token)
            {
                var expirationTime = issuedToken.CreateTime.AddSeconds(m_tokensExpirationConfiguration.TwoFactorTokenExpirationInSeconds);

                if (DateTime.UtcNow <= expirationTime)
                {
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}
