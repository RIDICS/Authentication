using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Authentication.Identity.TokenProviders;
using Ridics.Authentication.Service.Models;
using Ridics.Authentication.Service.Models.ViewModel.Users;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Service.Helpers
{
    public class TwoFactorValidator
    {
        private readonly ITranslator m_translator;
        private readonly IdentityUserManager m_identityUserManager;
        private readonly UserManager<ApplicationUser> m_userManager;
        private readonly Dictionary<string, TotpSecurityStampBasedTokenProvider<ApplicationUser>> m_tokenProviders;

        public TwoFactorValidator(ITranslator translator, IdentityUserManager identityUserManager, EmailTokenProvider emailProvider, SmsTokenProvider smsProvider, UserManager<ApplicationUser> manager)
        {
            m_translator = translator;
            m_identityUserManager = identityUserManager;
            m_userManager = manager;

            m_tokenProviders = new Dictionary<string, TotpSecurityStampBasedTokenProvider<ApplicationUser>>
            {
                {"Default", emailProvider}, //TODO make default provider configurable
                {EmailTokenProvider.ProviderName, emailProvider},
                {SmsTokenProvider.ProviderName, smsProvider},
            };
        }
        
        /// <summary>
        /// Checks updated contacts and two factor from userviewmodel.
        /// This method is used when user updates his contacts and also two factor during one request.
        /// </summary>
        /// <param name="userId">Id of user</param>
        /// <param name="userViewModel">Model containing updated contacts and two factor</param>
        /// <returns>True if user can use selected two factor provider with filled contacts, false and error description otherwise</returns>
        public async Task<TwoFactorResult> CheckTwoFactorIsValidOrNotEnabledAsync(int userId, UserViewModel userViewModel)
        {
            var appUser = await m_identityUserManager.GetUserByIdAsync(userId);

            appUser.PhoneNumber = userViewModel.PhoneNumber; //Set new contacts and check two factor with them
            appUser.Email = userViewModel.Email;

            return await CheckTwoFactorIsValidOrNotEnabledAsync(userViewModel.TwoFactorProvider, appUser);
        }

        /// <summary>
        /// Checks if app user can use selected two factor provider
        /// </summary>
        /// <param name="provider">Selected two factor provider</param>
        /// <param name="user">Application user</param>
        /// <returns>True if user can use two factor provider, false and error description otherwise</returns>
        public async Task<TwoFactorResult> CheckTwoFactorIsValidOrNotEnabledAsync(string provider, ApplicationUser user)
        {
            if (!user.TwoFactorEnabled)
            {
                return Success();
            }
            
            if (!m_tokenProviders.TryGetValue(provider, out var tokenProvider))
            {
                return Error(m_translator.Translate("invalid-provider"));
            }

            var result = await tokenProvider.CanGenerateTwoFactorTokenAsync(m_userManager, user);

            return result ? Success() : Error(m_translator.Translate("cannot-use-this-provider"));
        }

        private TwoFactorResult Error(string errorMessage = null)
        {
            return new TwoFactorResult
            {
                IsSuccessful = false,
                Message = string.IsNullOrEmpty(errorMessage) ? m_translator.Translate("error-occured") : errorMessage,
                Code = DataResultErrorCode.TwoFactorValidationError
            };
        }

        private TwoFactorResult Success()
        {
            return new TwoFactorResult
            {
                IsSuccessful = true
            };
        }
    }
}