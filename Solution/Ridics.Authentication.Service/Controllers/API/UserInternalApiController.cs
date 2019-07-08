using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.DataContracts.User;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Helpers;
using Ridics.Core.Shared.Types;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/user")]
    [JwtAuthorize(Policy = PolicyNames.InternalApiPolicy)]
    public class UserInternalApiV1Controller : ApiControllerBase
    {
        private readonly TwoFactorValidator m_twoFactorValidator;
        private readonly UserManager m_usersManager;
        private readonly ILogger m_logger;
        private readonly RoleManager m_roleManager;
        private readonly UserHelper m_userHelper;

        public UserInternalApiV1Controller(UserManager usersManager, IdentityUserManager identityUserManager,
            TwoFactorValidator twoFactorValidator, RoleManager roleManager, UserHelper userHelper,
            ILogger<UserInternalApiV1Controller> logger) : base(identityUserManager)
        {
            m_usersManager = usersManager;
            m_twoFactorValidator = twoFactorValidator;
            m_logger = logger; // Logger must be with specified generics in controller, otherwise is unresolved
            m_roleManager = roleManager;
            m_userHelper = userHelper;
        }

        [HttpGet("list")]
        [JwtAuthorize(Policy = PermissionNames.ListUsers)]
        [ProducesResponseType(typeof(ListContract<UserContract>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListUsersAsync([FromQuery] int start = 0, [FromQuery] int count = DefaultListCount, [FromQuery] string search = null)
        {
            if (count > MaxListCount)
            {
                count = MaxListCount;
            }

            var usersResult = m_usersManager.FindNonAuthenticationServiceUsers(start, count, search);
            var usersCountResult = m_usersManager.GetNonAuthenticationServiceUsersCount(search);
            
            if (usersResult.HasError)
            {
                return Error(usersResult.Error);
            }

            if (usersCountResult.HasError)
            {
                return Error(usersCountResult.Error);
            }

            var userContracts = Mapper.Map<IList<UserContract>>(usersResult.Result);

            await m_userHelper.FillValidTwoFactorProvidersAsync(userContracts);

            var contractList = new ListContract<UserContract>
            {
                Items = userContracts,
                ItemsCount = usersCountResult.Result,
            };

            Response.Headers.Add(HeaderStart, start.ToString());
            Response.Headers.Add(HeaderCount, count.ToString());

            return Json(contractList);
        }

        [HttpGet("{userid}")]
        [JwtAuthorize]
        [ProducesResponseType(typeof(UserContract), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAsync([FromRoute] [Required] int userid)
        {
            var authorized = await m_userHelper.IsUserAuthorizedAsync(userid, false, true, null, PermissionNames.EditAnyUsersData);

            if (!authorized)
            {
                return Forbid();
            }

            var userResult = m_usersManager.GetUserById(userid);

            if (userResult.HasError)
            {
                return Error(userResult.Error);
            }

            var userContract = Mapper.Map<UserContract>(userResult.Result);

            await m_userHelper.FillValidTwoFactorProvidersAsync(userContract);

            return Json(userContract);
        }

        [HttpGet("{muid}/full")]
        [ProducesResponseType(typeof(UserContractBase), StatusCodes.Status200OK)]
        public IActionResult GetUserByMuidAsync([FromRoute] [Required] Guid muid)
        {
            var userResult = m_usersManager.GetUserByDataType(UserDataTypes.MasterUserId, muid.ToString());

            if (userResult.HasError)
            {
                return Error(userResult.Error);
            }

            var userContract = Mapper.Map<UserContractBase>(userResult.Result);

            return Json(userContract);
        }

        [HttpGet("basic")]
        [ProducesResponseType(typeof(BasicUserInfoContract), StatusCodes.Status200OK)]
        public IActionResult GetUserBasicInfo([FromQuery] [Required] UserIdentifierTypeContract idType, [FromQuery] [Required] string id)
        {
            Core.Models.DataResult.DataResult<UserBasicInfoModel> userResult;
            switch (idType)
            {
                case UserIdentifierTypeContract.MasterUserId:
                    if (!Guid.TryParse(id, out _))
                    {
                        return Error("Invalid format of Master User ID");
                    }
                    userResult = m_usersManager.GetBasicUserInfoByDataType(UserDataTypes.MasterUserId, id);
                    break;
                default:
                    return BadRequest();
            }

            if (userResult.HasError)
            {
                return Error(userResult.Error);
            }

            var result = Mapper.Map<BasicUserInfoContract>(userResult.Result);
            return Json(result);
        }

        [HttpGet("basic/list")]
        [JwtAuthorize]
        [ProducesResponseType(typeof(IList<BasicUserInfoContract>), StatusCodes.Status200OK)]
        public IActionResult GetUsersBasicInfoAsync([FromQuery] [Required] List<Guid> muids)
        {
            var userResult = m_usersManager.FindBasicUserInfosByDataType(UserDataTypes.MasterUserId, muids.Select(x => x.ToString()).ToList());

            if (userResult.HasError)
            {
                return Error(userResult.Error);
            }

            var userContract = Mapper.Map<IList<BasicUserInfoContract>>(userResult.Result);

            return Json(userContract);
        }

        [HttpGet("contact")]
        [ProducesResponseType(typeof(IList<UserContactContract>), StatusCodes.Status200OK)]
        public IActionResult GetUserContacts([FromQuery] [Required] UserIdentifierTypeContract userIdType, [FromQuery] [Required] string idValue)
        {
            Core.Models.DataResult.DataResult<UserBasicInfoModel> userResult;
            switch (userIdType)
            {
                case UserIdentifierTypeContract.MasterUserId:
                    if (!Guid.TryParse(idValue, out _))
                    {
                        return Error("Invalid format of Master User ID");
                    }
                    userResult = m_usersManager.GetBasicUserInfoByDataType(UserDataTypes.MasterUserId, idValue);
                    break;
                default:
                    return BadRequest();
            }

            if (userResult.HasError)
            {
                return Error(userResult.Error);
            }

            var contacts = Mapper.Map<IList<UserContactContract>>(userResult.Result.UserContacts);
            
            return Json(contacts);
        }

        //This method was replaced with methods inside RegistrationController and is not used anywhere, maybe delete this method in future.
        [HttpGet("get")]
        [JwtAuthorize(Policy = PermissionNames.FindUserByRegistrationCode)]
        [ProducesResponseType(typeof(UserContract), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegisteredUserByVerificationCodeAsync([Required] string verificationCode)
        {
            if (string.IsNullOrEmpty(verificationCode))
            {
                return BadRequest();
            }

            var userResult = m_usersManager.GetUserByVerificationCode(verificationCode);

            if (userResult.HasError)
            {
                return Error(userResult.Error);
            }

            var userContract = Mapper.Map<UserContract>(userResult.Result);

            await m_userHelper.FillValidTwoFactorProvidersAsync(userContract);

            return Json(userContract);
        }

        [HttpPut("{id}/edit")]
        [JwtAuthorize(Policy = PermissionNames.EditAnyUsersData)]
        public async Task<IActionResult> PutEditedOtherUserAsync([Required] [FromRoute] int id,
            [Required] [FromBody] UserContract userContract)
        {
            return await EditUserAsync(id, userContract);
        }


        [HttpPut("{id}/editself")]
        [JwtAuthorize(Policy = PermissionNames.EditSelfPersonalData)]
        public async Task<IActionResult> PutEditedVerifiedUserEditedBySelfAsync([Required] [FromRoute] int id,
            [Required] [FromBody] UserContract userContract)
        {
            return await EditUserAsync(id, userContract);
        }

        //TODO refactor this method
        private async Task<IActionResult> EditUserAsync([Required] [FromRoute] int id,
            [Required] [FromBody] UserContract userContract)
        {
            var userResult = m_usersManager.GetUserById(id);

            if (userResult.HasError)
            {
                return Error(userResult.Error);
            }

            var user = userResult.Result;

            var verifiedUserRole = user.Roles.SingleOrDefault(x => x.Name == RoleNames.VerifiedUser);

            if (verifiedUserRole != null)
            {
                var removeRoleResult = m_usersManager.RemoveRoleFromUser(user.Id, verifiedUserRole.Id);

                if (removeRoleResult.HasError)
                {
                    return Error(removeRoleResult.Error);
                }

                var result = m_usersManager.AddRoleToUser(user.Id, RoleNames.RegisteredUser);

                if (result.HasError)
                {
                    return Error(result.Error);
                }

                var verificationCodeResult = m_usersManager.RegenerateVerificationCode(user.Id);

                if (verificationCodeResult.HasError)
                {
                    return Error(verificationCodeResult.Error);
                }
            }

            var userModel = Mapper.Map<UserModel>(userContract);
            //TODO check what can be updated
            user.UserData = userModel.UserData;

            var appUser = Mapper.Map<ApplicationUser>(user);

            var updateResult = await m_identityUserManager.UpdateAsync(id, appUser);

            if (updateResult.Succeeded)
            {
                return Ok();
            }

            var error = updateResult.Errors.FirstOrDefault();

            return Error(error?.Description, error?.Code);
        }

        [HttpPost("{id}/roles")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserRoles)]
        public IActionResult AssignRolesToUser([FromRoute] [Required] int id,
            [FromBody] [Required] IEnumerable<int> roleIds)
        {
            var result = m_usersManager.AssignRolesToUser(id, roleIds);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpPut("{id}/role/{roleId}")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserRoles)]
        public IActionResult AddRoleToUser([Required] int id, [Required] int roleId)
        {
            //var roleResult = m_roleManager.FindRoleById(roleId);

            //if (roleResult.HasError)
            //{
            //    return Error(roleResult.Error);
            //}

            //var role = roleResult.Result;

            if (User.HasPermission(PermissionNames.ManageUserRoles))
            {
                var result = m_usersManager.AddRoleToUser(id, roleId);

                if (result.HasError)
                {
                    return Error(result.Error);
                }

                return Ok();
            }

            return Forbid();
        }

        [HttpDelete("{id}/role/{roleId}")]
        [JwtAuthorize(Policy = PermissionNames.ManageUserRoles)]
        public IActionResult RemoveRoleFromUser([Required] int id, [Required] int roleId)
        {
            //var roleResult = m_roleManager.FindRoleById(roleId);

            //if (roleResult.HasError)
            //{
            //    return Error(roleResult.Error);
            //}

            //var role = roleResult.Result;

            if (User.HasPermission(PermissionNames.ManageUserRoles))
            {
                var result = m_usersManager.RemoveRoleFromUser(id, roleId);

                if (result.HasError)
                {
                    return Error(result.Error);
                }

                return Ok();
            }

            return Forbid();
        }
        
        [HttpDelete("{id}/delete")]
        [JwtAuthorize(Policy = PermissionNames.EditAnyUsersData)]
        public IActionResult Delete([FromRoute] [Required] int id)
        {
            if (m_logger.IsEnabled(LogLevel.Information))
            {
                m_logger.LogInformation(GetMethodCalledLoggingString($"id = {id}",null));
            }

            var result = m_usersManager.DeleteUserWithId(id);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpPost("{id}/claims")]
        [JwtAuthorize]
        public IActionResult AssignClaimsToUser(
            [FromRoute] [Required] int id,
            [FromBody] [Required] IEnumerable<int> claimIds
        )
        {
            var result = m_usersManager.AssignClaimsToUser(id, claimIds);

            if (result.HasError)
            {
                return Error(result.Error);
            }

            return Ok();
        }

        [HttpPost("{userId}/disconnectExternalLogin")]
        [JwtAuthorize]
        public async Task<IActionResult> DisconnectExternalLogin([Required] [FromRoute] int userId,
            [Required] [FromBody] ExternalLoginContract externalLoginContract)
        {
            var authorized = await m_userHelper.IsUserAuthorizedAsync(userId, false, false);

            if (!authorized)
            {
                return Forbid();
            }

            var resultDeleteLink = m_usersManager.DeleteExternalLoginByUser(userId, externalLoginContract.Id);

            if (resultDeleteLink.HasError)
            {
                return Error(resultDeleteLink.Error);
            }

            return Ok();
        }

        [HttpPost("{userId}/changePassword")]
        [JwtAuthorize]
        public async Task<IActionResult> ChangePassword([Required] [FromRoute] int userId,
            [Required] [FromBody] ChangePasswordContract contract)
        {
            var authorized = await m_userHelper.IsUserAuthorizedAsync(userId, false, false);

            if (!authorized)
            {
                return Forbid();
            }

            var user = await m_identityUserManager.FindByIdAsync(userId.ToString());
            var result = await m_identityUserManager.ChangePasswordAsync(user, contract.OriginalPassword, contract.Password);

            if (!result.Succeeded)
            {
                return Error(result.Errors.FirstOrDefault()?.Description, result.Errors.FirstOrDefault()?.Code);
            }

            return Ok();
        }

        //This method was replaced with methods inside ContactInternalApiController, maybe delete this method in future (It is called from portal from UserController.EditContact and EditSelfContact, however these methods are also obsolete) 
        [HttpPost("{userId}/changeContact")]
        [JwtAuthorize]
        public async Task<ActionResult> ChangeContact([FromRoute] [Required] int userId,
            [FromBody] [Required] ChangeUserContactsContract contract)
        {
            // TODO analyze if method is still valid, if so analyze also required permissions
            var authorized = await m_userHelper.IsUserAuthorizedAsync(userId, true, true, PermissionNames.EditSelfContacts, PermissionNames.EditAnyUsersData);

            if (!authorized)
            {
                return Forbid();
            }

            var user = await m_identityUserManager.GetUserByIdAsync(userId);

            if (user.Email != contract.NewEmailValue)
            {
                var result = await m_identityUserManager.ChangeEmailAsync(user, contract.NewEmailValue);
                if (result.HasError)
                {
                    return Error(result.Error);
                }
            }

            if (user.PhoneNumber != contract.NewPhoneNumberValue)
            {
                var result = await m_identityUserManager.ChangePhoneAsync(user, contract.NewPhoneNumberValue);
                if (result.HasError)
                {
                    return Error(result.Error);
                }
            }

            return Json(true);
        }

        [HttpPost("{userId}/setTwoFactor")]
        [JwtAuthorize]
        public async Task<IActionResult> SetTwoFactor([Required] [FromRoute] int userId,
            [Required] [FromBody] ChangeTwoFactorContract contract)
        {
            var authorized = await m_userHelper.IsUserAuthorizedAsync(userId, true, true, PermissionNames.SetTwoFactor, PermissionNames.EditAnyUsersData);

            if (!authorized)
            {
                return Forbid();
            }

            var user = await m_identityUserManager.FindByIdAsync(userId.ToString());

            var result = await m_identityUserManager.SetTwoFactorEnabledAsync(user, contract.TwoFactorIsEnabled);

            return !result.Succeeded ? Error(result.Errors.FirstOrDefault()?.Description, result.Errors.FirstOrDefault()?.Code) : Ok();
        }

        [HttpPost("{userId}/changeTwoFactorProvider")]
        [JwtAuthorize]
        public async Task<IActionResult> ChangeTwoFactorProvider([Required] [FromRoute] int userId,
            [Required] [FromBody] ChangeTwoFactorContract contract)
        {
            var authorized = await m_userHelper.IsUserAuthorizedAsync(userId, true, true, PermissionNames.SelectTwoFactorProvider, PermissionNames.EditAnyUsersData);

            if (!authorized)
            {
                return Forbid();
            }

            var user = await m_identityUserManager.FindByIdAsync(userId.ToString());

            var twoFactorCheckResult = await m_twoFactorValidator.CheckTwoFactorIsValidOrNotEnabledAsync(contract.TwoFactorProvider, user);

            if (!twoFactorCheckResult.IsSuccessful)
            {
                return Error(twoFactorCheckResult.Message, twoFactorCheckResult.Code);
            }

            var result = await m_identityUserManager.SetTwoFactorProviderAsync(user, contract.TwoFactorProvider);

            return !result.Succeeded ? Error(result.Errors.FirstOrDefault()?.Description, result.Errors.FirstOrDefault()?.Code) : Ok();
        }

        [HttpPost("{userId}/createVerificationRequest")]
        [JwtAuthorize]
        public async Task<IActionResult> CreateVerificationRequest([Required] [FromRoute] int userId)
        {
            var authorized = await m_userHelper.IsUserAuthorizedAsync(userId, false, false);

            if (!authorized)
            {
                return Forbid();
            }

            var result = m_usersManager.RegenerateVerificationCode(userId);
            if (result.HasError)
            {
                return Error(result.Error);
            }
            
            return Ok();
        }

        [HttpGet("{userId}/basic/roles")]
        [ProducesResponseType(typeof(UserWithRolesContract), StatusCodes.Status200OK)]
        [Authorize(Policy = PermissionNames.ManageUserRoles)]
        public IActionResult GetUserBasicWithRolesAsync([FromRoute] [Required] int userId)
        {
            var userResult = m_usersManager.GetUserById(userId);

            if (userResult.HasError)
            {
                return Error(userResult.Error);
            }

            var userContract = Mapper.Map<UserWithRolesContract>(userResult.Result);

            return Json(userContract);
        }

        [HttpGet("{userId}/role")]
        [ProducesResponseType(typeof(IList<RoleContractBase>), StatusCodes.Status200OK)]
        [JwtAuthorize]
        public IActionResult GetRolesByUser([FromRoute] [Required] int userId)
        {
            var rolesResult = m_roleManager.GetRolesByUser(userId);

            if (rolesResult.HasError)
            {
                return Error(rolesResult.Error);
            }

            var rolesContract = Mapper.Map<IList<RoleContractBase>>(rolesResult.Result);

            return Json(rolesContract);
        }
        
        [HttpGet("muid")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [JwtAuthorize]
        public IActionResult GetUserMuidAsync([FromQuery] [Required] UserIdentifierTypeContract idType, [FromQuery] [Required] string id)
        {
            Core.Models.DataResult.DataResult<UserBasicInfoModel> userResult;
            switch (idType)
            {
                case UserIdentifierTypeContract.MasterUserId:
                    return Json(id);
                case UserIdentifierTypeContract.DatabaseId:
                    userResult = m_usersManager.GetBasicUserInfoById(int.Parse(id));
                    break;
                default:
                    return BadRequest();
            }

            if (userResult.HasError)
            {
                if (userResult.Error.Code == DataResultErrorCode.UserNotExistId ||
                    userResult.Error.Code == DataResultErrorCode.UserNotExistUserData) //Return ok with empty content when user not found
                {
                    return Ok(); 
                }
                return Error(userResult.Error);
            }

            var muid = userResult.Result.UserData.First(x => x.UserDataType == UserDataTypes.MasterUserId)?.Value;
            return Json(muid);
        }
    }
}