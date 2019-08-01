using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.MessageSenders;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Exceptions;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Service.Authentication.Identity.Managers
{
    public class IdentityUserManager : UserManager<ApplicationUser>
    {
        private readonly UserManager m_userManager;
        private readonly UserContactManager m_userContactManager;
        private readonly MessageSenderManager m_messageSenderManager;
        private readonly ITranslator m_translator;
        private readonly LinkGenerator m_linkGenerator;

        private int m_maxGenerateResetPasswordTokenRetries = 10; //TODO make configurable
        
        public IdentityUserManager(UserManager userManager, UserContactManager userContactManager,
            MessageSenderManager messageSenderManager, IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger,
            ITranslator translator, LinkGenerator linkGenerator) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
                services, logger)
        {
            m_userManager = userManager;
            m_userContactManager = userContactManager;
            m_messageSenderManager = messageSenderManager;
            m_translator = translator;
            m_linkGenerator = linkGenerator;
        }

        public async Task<bool> IsInRoleAsync(int userId, string role)
        {
            var appUser = await GetUserByIdAsync(userId);
            var userRole = appUser.Roles.FirstOrDefault(x => x.Name == role);
            return userRole != null;
        }

        public Task<ApplicationUser> GetUserByIdAsync(int userId)
        {
            var user = m_userManager.GetUserById(userId).Result;
            var appUser = Mapper.Map<ApplicationUser>(user);
            return Task.FromResult(appUser);
        }

        public Task<ApplicationUser> GetUserByUsernameAsync(string userName)
        {
            var user = m_userManager.GetUserByUsername(userName).Result;
            var appUser = Mapper.Map<ApplicationUser>(user);
            return Task.FromResult(appUser);
        }

        public override async Task<string> GenerateTwoFactorTokenAsync(ApplicationUser user, string provider)
        {
            var token = await base.GenerateTwoFactorTokenAsync(user, provider);
            var userModel = Mapper.Map<UserModel>(user);
            m_userManager.CreateTwoFactorTokenForUser(userModel, provider, token);
            return token;
        }

        public void DeleteTwoFactorToken(ApplicationUser user, string provider)
        {
            m_userManager.DeleteTwoFactorTokenForUser(user.Id, provider);
        }

        public Task<TwoFactorLoginModel> GetTwoFactorTokenForUserAsync(ApplicationUser user, string provider)
        {
            var twoFactorLogin = m_userManager.GetTwoFactorTokenForUser(user.Id, provider).Result;
            return Task.FromResult(twoFactorLogin);
        }

        public async Task<IdentityResult> UpdateAsync(int userId, ApplicationUser user)
        {
            var origAppUser = await FindByIdAsync(userId.ToString());
            MergeUserViewModel(origAppUser, user);
            return await UpdateAsync(origAppUser);
        }

        private void MergeUserViewModel(ApplicationUser origAppUser, ApplicationUser updatedAppUser)
        {
            //TODO review what can and should be updateable!
            origAppUser.UserName = updatedAppUser.UserName;
            origAppUser.Email = updatedAppUser.Email;
            origAppUser.EmailConfirmed = updatedAppUser.EmailConfirmed;
            origAppUser.SecurityStamp = Guid.NewGuid().ToString();
            origAppUser.PhoneNumber = updatedAppUser.PhoneNumber;
            origAppUser.PhoneNumberConfirmed = updatedAppUser.PhoneNumberConfirmed;
            origAppUser.TwoFactorEnabled = updatedAppUser.TwoFactorEnabled;
            origAppUser.LockoutEnd = updatedAppUser.LockoutEnd;
            origAppUser.LockoutEnabled = updatedAppUser.LockoutEnabled;
            origAppUser.AccessFailedCount = updatedAppUser.AccessFailedCount;
            origAppUser.TwoFactorProvider = updatedAppUser.TwoFactorProvider;
            origAppUser.LastChange = updatedAppUser.LastChange;
            origAppUser.UserData = updatedAppUser.UserData;
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var code = m_userContactManager.GenerateEmailConfirmCode().Result;

            return Task.FromResult(code); //TODO specify action on error
        }

        public Task<string> GeneratePhoneConfirmationTokenAsync(ApplicationUser user)
        {
            var code = m_userContactManager.GeneratePhoneConfirmCode().Result;

            return Task.FromResult(code); //TODO specify action on error
        }

        public Task<bool> ConfirmEmailAsync(int userId, string confirmCode)
        {
            var result = m_userContactManager.ConfirmEmail(userId, confirmCode);

            if (result.HasError)
            {
                var exception = new ConfirmContactException
                {
                    Error = result.Error,
                };

                throw exception;
            }
            return Task.FromResult(result.Result);
        }

        public Task<bool> ConfirmPhoneAsync(int userId, string confirmCode)
        {
            var result = m_userContactManager.ConfirmPhone(userId, confirmCode);

            if (result.HasError)
            {
                var exception = new ConfirmContactException
                {
                    Error = result.Error,
                };

                throw exception;
            }

            return Task.FromResult(result.Result);
        }

        public async Task SendConfirmContactsCodesAsync(ApplicationUser user)
        {
            await SendConfirmCodeEmailAsync(user, user.EmailConfirmCode);
            await SendConfirmCodePhoneAsync(user, user.PhoneNumberConfirmCode);
        }

        public Task ResendConfirmCodeEmailAsync(ApplicationUser user)
        {
            var code = m_userContactManager.GetUserConfirmCodeEmail(user.Id).Result;
            return SendConfirmCodeEmailAsync(user, code);
        }

        public Task ResendConfirmCodePhoneNumberAsync(ApplicationUser user)
        {
            var code = m_userContactManager.GetUserConfirmCodePhoneNumber(user.Id).Result;
            return SendConfirmCodePhoneAsync(user, code);
        }

        public Task SendConfirmCodePhoneAsync(ApplicationUser user, string confirmCode)
        {
            return SendConfirmCodeAsync(user, confirmCode, MessageSenderType.SMS);
        }

        public Task SendConfirmCodeEmailAsync(ApplicationUser user, string confirmCode)
        {
            return SendConfirmCodeAsync(user, confirmCode, MessageSenderType.Email);
        }

        private Task SendConfirmCodeAsync(ApplicationUser user, string confirmCode, MessageSenderType senderType)
        {
            var userModel = Mapper.Map<UserModel>(user);

            m_messageSenderManager.SendMessage(userModel, senderType, m_translator.Translate("confirmation-code-subject"),
                string.Format(m_translator.Translate("confirmation-code-message"), user.UserName, confirmCode));

            return Task.CompletedTask;
        }

        public async Task<DataResult<bool>> ChangeEmailAsync(ApplicationUser user, string newValue)
        {
            var code = await GenerateEmailConfirmationTokenAsync(user);

            var result = m_userContactManager.ChangeEmail(user.Id, newValue, code);
            
            if (result.Succeeded && result.Result)
            {
                user.Email = newValue; //Better way would be to load formatted contact from database and update all contact related properties (confirmation, confirmcode etc), however in this use case it is not necessary, because the new value is used only for SendConfirmCode method 

                _ = SendConfirmCodeEmailAsync(user, code);//Ignore error, user can use Resend button
            }

            return result;
        }

        public async Task<DataResult<bool>> ChangePhoneAsync(ApplicationUser user, string newValue)
        {
            var code = await GeneratePhoneConfirmationTokenAsync(user);

            var result = m_userContactManager.ChangePhone(user.Id, newValue, code);

            if (result.Succeeded && result.Result)
            {
                user.PhoneNumber = newValue; //Better way would be to load formatted contact from database and update all contact related properties (confirmation, confirmcode etc), however in this use case it is not necessary, because the new value is used only for SendConfirmCode method 
                
                _ = SendConfirmCodePhoneAsync(user, code);//Ignore error, user can use Resend button
            }

            return result;
        }

        public override Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var user = m_userManager.GetUserByConfirmedEmail(email).Result;
            var appUser = Mapper.Map<ApplicationUser>(user);
            return Task.FromResult(appUser);
        }

        public override Task<IdentityResult> SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled)
        {
            return enabled ? TwoFactorEnableAsync(user) : TwoFactorDisableAsync(user);
        }

        public async Task<IdentityResult> SetTwoFactorProviderAsync(ApplicationUser user, string twoFactorProvider)
        {
            if (string.IsNullOrEmpty(twoFactorProvider) || !await IsTwoFactorProviderValidAsync(user, twoFactorProvider))
            {
                var validTwoFactorProviders = await GetValidTwoFactorProvidersAsync(user);

                twoFactorProvider = validTwoFactorProviders.FirstOrDefault();
            }

            var result = m_userManager.SetTwoFactorProvider(user.Id, twoFactorProvider);

            if (result.HasError)
            {
                var error = new IdentityError
                {
                    Description = result.Error.Message,
                    Code = result.Error.Code
                };

                return IdentityResult.Failed(error);
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> TwoFactorEnableAsync(ApplicationUser user)
        {
            var provider = user.TwoFactorProvider;

            if (string.IsNullOrEmpty(provider) || !await IsTwoFactorProviderValidAsync(user, provider))
            {
                var validTwoFactorProviders = await GetValidTwoFactorProvidersAsync(user);

                provider = validTwoFactorProviders.FirstOrDefault();
            }
            
            var result = m_userManager.EnableTwoFactorAuth(user.Id, provider);

            if (result.HasError)
            {
                var error = new IdentityError
                {
                    Description = result.Error.Message,
                    Code = result.Error.Code
                };

                return IdentityResult.Failed(error);
            }
            return IdentityResult.Success;
        }

#pragma warning disable 1998
        public async Task<IdentityResult> TwoFactorDisableAsync(ApplicationUser user)
#pragma warning restore 1998
        {

            var result = m_userManager.DisableTwoFactorAuth(user.Id);

            if (result.HasError)
            {
                var error = new IdentityError
                {
                    Description = result.Error.Message,
                    Code = result.Error.Code
                };

                return IdentityResult.Failed(error);
            }

            return IdentityResult.Success;
        }

        public async Task<bool> IsTwoFactorProviderValidAsync(ApplicationUser user, string provider)
        {
            var validTwoFactorProviders = await GetValidTwoFactorProvidersAsync(user);

            var validProvider = validTwoFactorProviders.FirstOrDefault(x => x == provider);

            return !string.IsNullOrEmpty(validProvider);
        }

        public async Task<ApplicationUser> FindByNameOrEmailAsync(string usernameOrEmail)
        {
            if (string.IsNullOrEmpty(usernameOrEmail))
            {
                return null;
            }

            var user = await  FindByEmailAsync(usernameOrEmail) ?? await FindByNameAsync(usernameOrEmail);
            return user;
        }

        public async Task SendResetPasswordAsync(IUrlHelper url, ApplicationUser user, string httpProtocol)
        {
            var token = await GeneratePasswordResetTokenAsync(user);
            var culture = m_translator.GetRequestCulture().Name;

            var resetLink = url.Action( "ResetPassword", "Account",
                new {username = user.UserName, token = token, culture = culture}, httpProtocol);

            var userModel = Mapper.Map<UserModel>(user);

            m_messageSenderManager.SendMessage(userModel, MessageSenderType.Email,
                m_translator.Translate("reset-password-subject", "RequestResetPasswordViewModel"),
                string.Format(m_translator.Translate("reset-password-message", "RequestResetPasswordViewModel"), resetLink));
        }

        public override async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            var userModel = Mapper.Map<UserModel>(user);

            var generateRetries = 0;

            while (generateRetries <= m_maxGenerateResetPasswordTokenRetries)
            {
                var token = await base.GeneratePasswordResetTokenAsync(user);

                var result = m_userManager.SavePasswordResetToken(userModel, token);

                if (result.Succeeded)
                {
                    return token;
                }
            }

            throw new GenerateResetPasswordTokenException();
        }

        public override Task<ApplicationUser> FindByNameAsync(string userName)
        {
            var user = m_userManager.GetUserByUsername(userName).Result;
            var appUser = Mapper.Map<ApplicationUser>(user);
            return Task.FromResult(appUser);
        }
    }
}