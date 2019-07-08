using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Authentication.Identity.TokenProviders;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Exceptions;
using Ridics.Authentication.Service.Helpers;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/contact")]
    [JwtAuthorize(Policy = PolicyNames.InternalApiPolicy)]
    public class ContactInternalApiV1Controller : ApiControllerBase
    {
        private readonly ILogger<RegistrationInternalApiV1Controller> m_logger;
        private readonly TwoFactorValidator m_twoFactorValidator;

        public ContactInternalApiV1Controller(IdentityUserManager identityUserManager, ILogger<RegistrationInternalApiV1Controller> logger, TwoFactorValidator twoFactorValidator) : base(identityUserManager)
        {
            m_logger = logger;
            m_twoFactorValidator = twoFactorValidator;
        }

        /// <summary>
        /// Confirms contact and enables two factor if confirmation was successful
        /// </summary>
        /// <param name="contract">Info about contact and verification code</param>
        /// <returns>Json with boolean whether confirmation was successful</returns>
        [HttpPost("confirmContact")]
        public async Task<ActionResult> ConfirmContact([FromBody] [Required] ConfirmContactContract contract)
        {
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                m_logger.LogInformation(GetMethodCalledLoggingString(null, contract));
            }

            var success = false;

            if (contract.ContactType == ContactTypeEnum.Email)
            {
                try
                {
                    success = await m_identityUserManager.ConfirmEmailAsync(contract.UserId, contract.ConfirmCode);
                }
                catch (ConfirmContactException e)
                {
                    return Error(e.Error);
                }


                if (success)
                {
                    var user = await m_identityUserManager.GetUserByIdAsync(contract.UserId);

                    await SetTwoFactorAsync(user, ContactTypeEnum.Email);
                }
            }

            if (contract.ContactType == ContactTypeEnum.Phone)
            {
                try
                {
                    success = await m_identityUserManager.ConfirmPhoneAsync(contract.UserId, contract.ConfirmCode);
                }
                catch (ConfirmContactException e)
                {
                    return Error(e.Error);
                }
                

                if (success)
                {
                    var user = await m_identityUserManager.GetUserByIdAsync(contract.UserId);

                    await SetTwoFactorAsync(user, ContactTypeEnum.Phone);
                }
            }

            return Json(success);
        }

        /// <summary>
        /// Changes specific contact to new value and generates new confirmation code and sends it to new contact value
        /// </summary>
        /// <param name="contract">Info about contact to be changed and new value</param>
        /// <returns>Json with boolean whether contact change was successful</returns>
        [HttpPost("changeContact")]
        public async Task<ActionResult> ChangeContactAsync([FromBody] [Required] ChangeContactContract contract)
        {
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                m_logger.LogInformation(GetMethodCalledLoggingString(null, contract));
            }

            DataResult<bool> changeResult = null;

            var user = await m_identityUserManager.GetUserByIdAsync(contract.UserId);

            if (contract.ContactType == ContactTypeEnum.Email && !string.Equals(contract.NewContactValue, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                changeResult = await m_identityUserManager.ChangeEmailAsync(user, contract.NewContactValue);

                if (changeResult.HasError)
                {
                    return Error(changeResult.Error);
                }

                if (changeResult.Result)
                {
                    user = await m_identityUserManager.GetUserByIdAsync(contract.UserId); //Reload user with updated email related properties

                    AdjustTwoFactorOnEmailChange(user);
                }
            }

            if (contract.ContactType == ContactTypeEnum.Phone && contract.NewContactValue != user.PhoneNumber)
            {
                changeResult = await m_identityUserManager.ChangePhoneAsync(user, contract.NewContactValue);

                if (changeResult.HasError)
                {
                    return Error(changeResult.Error);
                }

                if (changeResult.Result)
                {
                    user = await m_identityUserManager.GetUserByIdAsync(contract.UserId); //Reload user with updated phone related properties

                    AdjustTwoFactorOnPhoneChange(user);
                }
            }

            //If change result is null, new contact value is same as original value (this should not happen, because there is check on portal before sending request for contact change), return true as if everything went ok 
            return Json(changeResult?.Result ?? true); 
        }

        /// <summary>
        /// Resend confirmation code to contact
        /// </summary>
        /// <param name="contract">Info about contact to which resend code </param>
        /// <returns>Json with boolean whether resend was successful</returns>
        [HttpPost("resendCode")]
        public async Task<ActionResult> ResendCode([FromBody] [Required] ContactContract contract)
        {
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                m_logger.LogInformation(GetMethodCalledLoggingString(null, contract));
            }

            var user = await m_identityUserManager.GetUserByIdAsync(contract.UserId);

            if (contract.ContactType == ContactTypeEnum.Email)
            {
                await m_identityUserManager.ResendConfirmCodeEmailAsync(user);
            }
            else if (contract.ContactType == ContactTypeEnum.Phone)
            {
                await m_identityUserManager.ResendConfirmCodePhoneNumberAsync(user);
            }

            return Json(true);
        }

        /// <summary>
        /// Sets two factor after contact confirmation -> If confirmed contact type was phone, set it as 2 factor provider. In every case enable two factor.
        /// </summary>
        /// <param name="user">User to set 2factor</param>
        /// <param name="type">Confirmed contact type</param>
        /// <returns>Task</returns>
        private async Task SetTwoFactorAsync(ApplicationUser user, ContactTypeEnum type)
        {
            if (type == ContactTypeEnum.Phone)//WORKAROUND make default provider configurable
            {
                user.TwoFactorProvider = SmsTokenProvider.ProviderName;
            }
            else
            {
                if (!user.TwoFactorEnabled && type == ContactTypeEnum.Email)
                {
                    user.TwoFactorProvider = EmailTokenProvider.ProviderName;
                }
            }

            await m_identityUserManager.TwoFactorEnableAsync(user);
        }

        /// <summary>
        /// Adjusts two factor after change of email (email is default provider)
        /// </summary>
        /// <param name="user">User to adjust 2factor</param>
        private async void AdjustTwoFactorOnEmailChange(ApplicationUser user)
        {
            //TODO make default configurable
            if (user.TwoFactorProvider != EmailTokenProvider.ProviderName && user.TwoFactorProvider != "Default") return;

            await AdjustTwoFactor(user, EmailTokenProvider.ProviderName, SmsTokenProvider.ProviderName);
        }

        /// <summary>
        /// Adjust two factor after change of phone
        /// </summary>
        /// <param name="user">User to adjust 2factor</param>
        private async void AdjustTwoFactorOnPhoneChange(ApplicationUser user)
        {
            if (user.TwoFactorProvider != SmsTokenProvider.ProviderName) return;

            await AdjustTwoFactor(user, SmsTokenProvider.ProviderName, EmailTokenProvider.ProviderName);
        }

        /// <summary>
        /// Adjust two factor after contact change. Check if current two factor provider is valid, if no check whether other two factor provider is valid, if yes set it as two factor provider, if not disable two factor.
        /// </summary>
        /// <param name="user">User to adjust 2factor</param>
        /// <param name="currentProviderName">Name of current 2factor provider</param>
        /// <param name="possibleNewProviderName">Name of new provider when current provider is not valid</param>
        /// <returns>Async task</returns>
        private async Task AdjustTwoFactor(ApplicationUser user, string currentProviderName, string possibleNewProviderName)
        {
            var currentProviderResult = await m_twoFactorValidator.CheckTwoFactorIsValidOrNotEnabledAsync(currentProviderName, user);

            if (!currentProviderResult.IsSuccessful)
            {
                var newProviderResult = await m_twoFactorValidator.CheckTwoFactorIsValidOrNotEnabledAsync(possibleNewProviderName, user);

                if (newProviderResult.IsSuccessful)
                {
                    user.TwoFactorProvider = possibleNewProviderName;

                    await m_identityUserManager.TwoFactorEnableAsync(user);
                }
                else
                {
                    await m_identityUserManager.TwoFactorDisableAsync(user);
                }
            }
        }
    }
}