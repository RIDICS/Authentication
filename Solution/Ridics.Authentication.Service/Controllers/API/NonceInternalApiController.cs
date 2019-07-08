using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authorization;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/nonce")]
    [JwtAuthorize(Policy = PolicyNames.NonceApiPolicy)]
    public class NonceInternalApiV1Controller : ApiControllerBase
    {
        private readonly NonceManager m_nonceManager;

        public NonceInternalApiV1Controller(
            NonceManager nonceManager,
            IdentityUserManager identityUserManager
        ) : base(identityUserManager)
        {
            m_nonceManager = nonceManager;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(NonceContract), StatusCodes.Status200OK)]
        public IActionResult Create([FromBody] NonceContract nonceContract)
        {
            m_nonceManager.HydrateNonceContract(nonceContract);

            return Json(nonceContract);
        }
    }
}
