using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.DataContracts.User;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Shared;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/registration")]
    [JwtAuthorize(Policy = PolicyNames.InternalApiPolicy)]
    public class RegistrationInternalApiV1Controller : ApiControllerBase
    {
        private readonly UserManager m_userManager;
        private readonly ILogger<RegistrationInternalApiV1Controller> m_logger;
        private readonly IPasswordValidator<ApplicationUser> m_passwordManager;
        private readonly ITranslator m_translator;

        private int m_maxGeneratePasswordRetries = 10; //TODO make configurable

        public RegistrationInternalApiV1Controller(IdentityUserManager identityUserManager, UserManager userManager,
            ILogger<RegistrationInternalApiV1Controller> logger,
            IPasswordValidator<ApplicationUser> passwordManager, ITranslator translator) : base(identityUserManager)
        {
            m_userManager = userManager;
            m_logger = logger; // Logger must be with specified generics in controller, otherwise is unresolved
            m_passwordManager = passwordManager;
            m_translator = translator;
        }

        [HttpPost("create")]
        [JwtAuthorize]
        public async Task<ActionResult> CreateUser([FromBody] [Required] CreateUserContract createUserContract)
        {
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                var passwordBackup = createUserContract.Password;
                createUserContract.Password = "*REMOVED*"; // Password can not be logged!

                m_logger.LogInformation(GetMethodCalledLoggingString(null, createUserContract));
                createUserContract.Password = passwordBackup;
            }

            var userModel = Mapper.Map<UserModel>(createUserContract.User);

            userModel.Username = string.IsNullOrEmpty(createUserContract.UserName)
                ? m_userManager.GenerateUsername().Result
                : createUserContract.UserName;

            var appUser = Mapper.Map<ApplicationUser>(userModel);

            appUser.EmailConfirmCode = await m_identityUserManager.GenerateEmailConfirmationTokenAsync(appUser);
            appUser.PhoneNumberConfirmCode = await m_identityUserManager.GeneratePhoneConfirmationTokenAsync(appUser);

            var result = await m_identityUserManager.CreateAsync(appUser, createUserContract.Password);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();
                return Error(error?.Description, error?.Code);
            }

            var user = m_identityUserManager.FindByNameAsync(appUser.UserName).Result;

            _ = m_identityUserManager.SendConfirmContactsCodesAsync(user); //Ignore error, user can use Resend button

            m_userManager.AddRoleToUser(user.Id, RoleNames.RegisteredUser);

            var userResult = m_userManager.GetUserByUsername(appUser.UserName);

            var userContract = Mapper.Map<UserContract>(userResult.Result);

            return Json(userContract);
        }

        [HttpPost("createVerified")]
        [JwtAuthorize(Policy = PermissionNames.RegisterOtherUsers)]
        public async Task<ActionResult> CreateVerifiedUserAsync([FromBody] [Required] CreateVerifiedUserContract createUserContract)
        {
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                m_logger.LogInformation(GetMethodCalledLoggingString(null, createUserContract));
            }

            var userModel = Mapper.Map<UserModel>(createUserContract.User);

            userModel.Username = string.IsNullOrEmpty(createUserContract.UserName)
                ? m_userManager.GenerateUsername().Result
                : createUserContract.UserName;

            var appUser = Mapper.Map<ApplicationUser>(userModel);

            var password = await GeneratePasswordForUserAsync(appUser);

            if (password == null)
            {
                return Error(m_translator.Translate(m_translator.Translate("generate-password-failed"),
                    DataResultErrorCode.GeneratePassword));
            }

            appUser.EmailConfirmCode = await m_identityUserManager.GenerateEmailConfirmationTokenAsync(appUser);
            appUser.PhoneNumberConfirmCode = await m_identityUserManager.GeneratePhoneConfirmationTokenAsync(appUser);

            var result = await m_identityUserManager.CreateAsync(appUser, password);

            if (!result.Succeeded)
            {
                var error = result.Errors.FirstOrDefault();
                return Error(error?.Description, error?.Code);
            }

            var user = m_identityUserManager.FindByNameAsync(appUser.UserName).Result;

            m_userManager.AddRoleToUser(user.Id, RoleNames.VerifiedUser);

            _ = m_identityUserManager.SendConfirmContactsCodesAsync(user); //Ignore error, user can use Resend button

            var userResult = m_userManager.GetUserByUsername(appUser.UserName).Result;

            userResult.VerificationCode = null;
            m_userManager.UpdateUser(userResult.Id,
                userResult); //TODO Why is here used this method instead of identity method for user update?

            var userContract = Mapper.Map<VerifiedUserCreatedContract>(userResult);

            userContract.Password = password;

            return Json(userContract);
        }

        [HttpGet("search")]
        [JwtAuthorize(Policy = PermissionNames.FindUserByRegistrationCode)]
        public ActionResult SearchUser([FromQuery] [Required] string code)
        {
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                m_logger.LogInformation(GetMethodCalledLoggingString($"code={code}", null));
            }

            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Parameter code can not be null");
            }

            var result = m_userManager.GetUserByVerificationCode(code);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            var userContract = Mapper.Map<UserContract>(result.Result);

            return Json(userContract);
        }

        [HttpGet("searchinfo")]
        [JwtAuthorize]
        public ActionResult SearchUserInfo([FromQuery] [Required] string code)
        {
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                m_logger.LogInformation(GetMethodCalledLoggingString($"code={code}", null));
            }

            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Parameter code can not be null");
            }

            var result = m_userManager.GetUserByVerificationCode(code);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            var userContract = Mapper.Map<UserInfoContract>(result.Result);

            return Json(userContract);
        }

        [HttpPost("verifyUser")]
        [JwtAuthorize(Policy = PermissionNames.ValidateUser)]
        public async Task<IActionResult> VerifyUser([FromBody] [Required] UserContractBase userContractBase,
            [FromQuery] [Required] string code)
        {
            var rolesToAttain = new[] {RoleNames.VerifiedUser};

            return await VerifyPortalUser<UserContractBase, UserContract>(userContractBase, code, rolesToAttain);
        }
        
        /// <summary>
        /// Verifies portal user. Sets specified role, set user data to verified
        /// </summary>
        /// <typeparam name="TS">Type of source contract - e.g. <see cref="UserContractBase"/></typeparam>
        /// <typeparam name="TD">Type of destination contract - e.g. <see cref="UserContract"/></typeparam>
        /// <param name="sourceContract">Contract containing user data entered or checked by verifier</param>
        /// <param name="verificationCode">Verification code for looking up user</param>
        /// <param name="roleNamesToReceive">Names of user roles that user shall receive</param>
        /// <returns>Json containing contract of type <typeparamref name="TD"/> on success, error status codes otherwise</returns>
        private async Task<IActionResult> VerifyPortalUser<TS, TD>(TS sourceContract, string verificationCode,
            IEnumerable<string> roleNamesToReceive) where TS : IConvertibleToUserData, IConvertibleToUserContacts
        {
            //TODO refactor this method
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                m_logger.LogInformation(GetMethodCalledLoggingString($"code={verificationCode}", sourceContract));
            }

            if (string.IsNullOrEmpty(verificationCode))
            {
                return BadRequest("Parameter code can not be null");
            }

            var result = m_userManager.GetUserByVerificationCode(verificationCode);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            var loadedUser = result.Result;

            //This mapping is only for resolving UserData
            var userInput = Mapper.Map<TS, UserModel>(sourceContract);

            //Update user info and user data
            loadedUser.UserData = userInput.UserData;

            //Update only contact values
            var emailContact = loadedUser.UserContacts.FirstOrDefault(x => x.Type == ContactTypeEnumModel.Email);
            var phoneContact = loadedUser.UserContacts.FirstOrDefault(x => x.Type == ContactTypeEnumModel.Phone);

            var appUser = Mapper.Map<ApplicationUser>(loadedUser);

            var sendConfirmEmail = await SetNewEmailContactAsync(appUser, emailContact, sourceContract);
            var sendConfirmSms = await SetNewPhoneContactAsync(appUser, phoneContact, sourceContract);

            var updateResult =
                m_userManager.UpdateUser(loadedUser.Id,
                    loadedUser); //TODO Why is here used this method instead of identity method for user update?

            if (updateResult.HasError)
            {
                return Error(updateResult.Error);
            }

            if (sendConfirmEmail)
            {
                _ = m_identityUserManager.SendConfirmCodeEmailAsync(appUser,
                    appUser.EmailConfirmCode); //Ignore error, user can use Resend button
            }

            if (sendConfirmSms)
            {
                _ = m_identityUserManager.SendConfirmCodePhoneAsync(appUser,
                    appUser.PhoneNumberConfirmCode); //Ignore error, user can use Resend button
            }

            var userDataVerifiedResult = m_userManager.SetUserDataVerified(loadedUser.Id, int.Parse(GetUserId()));

            if (userDataVerifiedResult.HasError)
            {
                return Error(userDataVerifiedResult.Error);
            }

            m_userManager.SwitchUserRoles(loadedUser.Id, new[] {RoleNames.RegisteredUser}, roleNamesToReceive);

            var userResult = m_userManager.GetUserById(loadedUser.Id);
            var userContract = Mapper.Map<TD>(userResult.Result);

            return Json(userContract);
        }

        private async Task<bool> SetNewPhoneContactAsync(ApplicationUser appUser, UserContactModel phoneContact,
            IConvertibleToUserContacts userContracts)
        {
            if (phoneContact != null && phoneContact.Value != userContracts.PhoneNumber
            ) //TODO It can happen that this check wont work before new contact is formatted, move this to manager
            {
                phoneContact.Value = userContracts.PhoneNumber;
                phoneContact.ConfirmCode = await m_identityUserManager.GeneratePhoneConfirmationTokenAsync(appUser);

                appUser.PhoneNumber = phoneContact.Value; //Save new values to appuser instance for future send of confirm code
                appUser.PhoneNumberConfirmCode = phoneContact.ConfirmCode;

                return true;
            }

            return false;
        }

        private async Task<bool> SetNewEmailContactAsync(ApplicationUser appUser, UserContactModel emailContact,
            IConvertibleToUserContacts userContracts)
        {
            if (emailContact != null && emailContact.Value != userContracts.Email
            ) //TODO It can happen that this check wont work before new contact is formatted, move this to manager
            {
                emailContact.Value = userContracts.Email;
                emailContact.ConfirmCode = await m_identityUserManager.GenerateEmailConfirmationTokenAsync(appUser);

                appUser.Email = emailContact.Value; //Save new values to appuser instance for future send of confirm code
                appUser.EmailConfirmCode = emailContact.ConfirmCode;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates password for user. Generating until random password is valid against specified password policy or until it reaches max retries
        /// </summary>
        /// <param name="appUser">User</param>
        /// <returns>Generated password or null if reaches maximum number of retries</returns>
        private async Task<string> GeneratePasswordForUserAsync(ApplicationUser appUser)
        {
            var generateRetries = 0;
            while (generateRetries <= m_maxGeneratePasswordRetries)
            {
                var password = m_userManager.GeneratePassword().Result;
                var passResult = await m_passwordManager.ValidateAsync(m_identityUserManager, appUser, password);

                if (passResult.Succeeded)
                {
                    return password;
                }

                generateRetries++;
            }

            return null;
        }
    }
}