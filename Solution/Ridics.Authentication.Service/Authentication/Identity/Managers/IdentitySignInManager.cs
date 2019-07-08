using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Authentication.Identity.TokenProviders;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Service.Authentication.Identity.Managers
{
    public class IdentitySignInManager : SignInManager<ApplicationUser>
    {
        private readonly IdentityUserManager m_userManager;
        private readonly MessageSenderManager m_messageSenderManager;
        private readonly MessageSenderTokenProviderAssociationManager m_messageSenderTokenProviderAssociationManager;
        private readonly ITranslator m_translator;
        private readonly TokenOptions m_tokenOptions;

        private readonly string m_defaultProvider;

        public IdentitySignInManager(
            IdentityUserManager userManager, MessageSenderManager messageSenderManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            MessageSenderTokenProviderAssociationManager messageSenderTokenProviderAssociationManager,
            IOptions<IdentityOptions> identityOptions,
            ITranslator translator
        ) : base(
            userManager, contextAccessor,
            claimsFactory, optionsAccessor, logger, schemes)
        {
            m_userManager = userManager;
            m_messageSenderManager = messageSenderManager;
            m_messageSenderTokenProviderAssociationManager = messageSenderTokenProviderAssociationManager;
            m_translator = translator;
            m_tokenOptions = identityOptions.Value.Tokens;
            m_defaultProvider = ResolveDefaultTokenProvider();
        }

        public string ResolveDefaultTokenProvider()
        {
            if (m_tokenOptions.ProviderMap.TryGetValue(TokenOptions.DefaultProvider, out var tokenProviderDescriptor))
            {
                if (tokenProviderDescriptor.ProviderType == typeof(EmailTokenProvider))
                {
                    return EmailTokenProvider.ProviderName;
                }

                if (tokenProviderDescriptor.ProviderType == typeof(SmsTokenProvider))
                {
                    return SmsTokenProvider.ProviderName;
                }
            }

            return EmailTokenProvider.ProviderName; //TODO HACK make configurable
        }

        public async Task ReloginUserAsync(int userId, bool isPersistent)
        {
            var user = m_userManager.GetUserByIdAsync(userId).Result;

            await SignOutAsync();
            await SignInAsync(user, isPersistent);
        }

        public async Task<SignInResult> TwoFactorSignInAsync(
            ApplicationUser user, string twoFactorCode, bool rememberMe,
            bool rememberMachine, string provider = null
        )
        {
            provider = GetProvider(provider);

            return await TwoFactorSignInAsync(provider, twoFactorCode, rememberMe, rememberMachine);
        }

        public async Task GenerateTwoFactorTokenAsync(ApplicationUser user, string provider = null)
        {
            provider = GetProvider(provider);

            var token = await m_userManager.GenerateTwoFactorTokenAsync(user, provider);
            var senderType = m_messageSenderTokenProviderAssociationManager.GetSenders(provider);

            var userModel = Mapper.Map<UserModel>(user);

            m_messageSenderManager.SendMessage(userModel, senderType,
                m_translator.Translate("login-security-code-subject"),
                string.Format(m_translator.Translate("login-security-code-message"), user.UserName, token));
        }

        public void DeleteTwoFactorToken(ApplicationUser user, string provider = null)
        {
            provider = GetProvider(provider);

            m_userManager.DeleteTwoFactorToken(user, provider);
        }

        /// <summary>
        /// Returns specific provider name based on <paramref name="provider"/>.
        /// </summary>
        /// <param name="provider">Provided provider</param>
        /// <returns>If <paramref name="provider"/> is null, empty or "Default" returns m_defaultProvider that is set in constructor. In other cases returns <paramref name="provider"/></returns>
        private string GetProvider(string provider)
        {
            if (string.IsNullOrEmpty(provider) || provider == TokenOptions.DefaultProvider)
            {
                return m_defaultProvider;
            }

            return provider;
        }
    }
}