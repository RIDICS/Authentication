using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Factories;
using Ridics.Authentication.Service.Models.API.UserActivation;
using Ridics.Core.Shared.Types;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/users")]
    [RequireApiAccessToken(ApiAccessPermissionEnumModel.UserActivation)]
    public class UsersExternalApiV1Controller : ApiControllerBase
    {
        private readonly UserManager m_userManager;
        private readonly UserApiResultFactory m_userApiResultFactory;

        public UsersExternalApiV1Controller(IdentityUserManager identityUserManager, UserManager userManager, UserApiResultFactory userApiResultFactory) : base(identityUserManager)
        {
            m_userManager = userManager;
            m_userApiResultFactory = userApiResultFactory;
        }

        [HttpGet("activationStatus")]
        [ProducesResponseType(typeof(List<UserActivationContainerResponse>), StatusCodes.Status200OK)]
        public IActionResult GetActivationStatusList([FromQuery] IdentifierType? idType, [FromQuery] IList<string> id)
        {
            if (idType == null || id == null)
            {
                return BadRequest();
            }
            if (id.Count > MaxListCount)
            {
                return BadRequest($"Max ID count is {MaxListCount}");
            }

            var resultList = new List<UserActivationContainerResponse>();

            foreach (var identifier in id)
            {
                var resultItem = new UserActivationContainerResponse
                {
                    IdFromRequest = identifier,
                    ActivationStatus = null,
                };
                resultList.Add(resultItem);

                var userResult = m_userManager.GetUserByDataType(UserDataTypes.MasterUserId, identifier);
                if (userResult.HasError)
                {
                    continue;
                }
                
                resultItem.ActivationStatus = m_userApiResultFactory.CreateResultForLastName(userResult.Result);
            }
            
            return Json(resultList);
        }
    }
}