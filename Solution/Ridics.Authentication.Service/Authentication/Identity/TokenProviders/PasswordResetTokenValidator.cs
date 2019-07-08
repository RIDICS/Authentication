using System;
using System.Threading.Tasks;
using Ridics.Authentication.Service.Configuration;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;

namespace Ridics.Authentication.Service.Authentication.Identity.TokenProviders
{
    public class PasswordResetTokenValidator
    {
        private readonly UserManager m_userManager;
        private readonly TokensExpirationConfiguration m_tokensExpirationConfiguration;

        public PasswordResetTokenValidator(UserManager userManager, TokensExpirationConfiguration tokensExpirationConfiguration)
        {
            m_userManager = userManager;
            m_tokensExpirationConfiguration = tokensExpirationConfiguration;
        }

        public Task<bool> ValidateAsync(string token, ApplicationUser user)
        {
            var userModel = m_userManager.GetUserById(user.Id);

            if (userModel.HasError)
            {
                return Task.FromResult(false);
            }

            var userToken = userModel.Result.PasswordResetToken;
            var userTokenCreateTime = userModel.Result.PasswordResetTokenCreateTime;

            if (!string.IsNullOrEmpty(token) && token == userToken && userTokenCreateTime.HasValue)
            {
                var expirationTime = userTokenCreateTime.Value.AddSeconds(m_tokensExpirationConfiguration.PasswordResetTokenExpirationInSeconds);

                if (DateTime.UtcNow <= expirationTime)
                {
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}
