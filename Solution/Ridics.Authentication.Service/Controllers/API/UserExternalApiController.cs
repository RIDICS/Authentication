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
    [Route(ApiRouteStart + "/v{version:apiVersion}/user")]
    [RequireApiAccessToken(ApiAccessPermissionEnumModel.UserActivation)]
    public class UserExternalApiV1Controller : ApiControllerBase
    {
        private readonly UserManager m_userManager;
        private readonly UserApiResultFactory m_userApiResultFactory;

        public UserExternalApiV1Controller(IdentityUserManager identityUserManager, UserManager userManager, UserApiResultFactory userApiResultFactory) : base(identityUserManager)
        {
            m_userManager = userManager;
            m_userApiResultFactory = userApiResultFactory;
        }

        [HttpGet("activationStatus")]
        [ProducesResponseType(typeof(UserActivationResponse), StatusCodes.Status200OK)]
        public IActionResult GetActivationStatus([FromQuery] IdentifierType? idType, [FromQuery] string id)
        {
            if (idType == null || string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var userResult = m_userManager.GetUserByDataType(UserDataTypes.MasterUserId, id);
            if (userResult.HasError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, userResult.Error);
            }

            var result = m_userApiResultFactory.CreateResultForLastName(userResult.Result);
            return Json(result);
        }
    }
}