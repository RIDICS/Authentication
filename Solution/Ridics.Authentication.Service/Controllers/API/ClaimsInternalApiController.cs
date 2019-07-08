using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/claims")]
    [JwtAuthorize(Policy = PolicyNames.InternalApiPolicy)]
    public class ClaimsInternalApiV1Controller : ApiControllerBase
    {
        private readonly ClaimTypeManager m_claimTypeManager;

        public ClaimsInternalApiV1Controller(
            ClaimTypeManager claimTypeManager, IdentityUserManager identityUserManager
        ) : base(identityUserManager)
        {
            m_claimTypeManager = claimTypeManager;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IList<ClaimTypeViewModel>), StatusCodes.Status200OK)]
        public IActionResult ListClaims([FromQuery] int start = 0, [FromQuery] int count = DefaultListCount)
        {
            if (count > MaxListCount)
            {
                count = MaxListCount;
            }

            var claimTypesResult = m_claimTypeManager.GetClaimTypes(start, count);

            if (claimTypesResult.HasError)
            {
                return BadRequest(claimTypesResult.Error);
            }

            var claimTypeViewModels = Mapper.Map<IList<ClaimTypeViewModel>>(claimTypesResult.Result);

            Response.Headers.Add(HeaderStart, start.ToString());
            Response.Headers.Add(HeaderCount, count.ToString());

            return Json(claimTypeViewModels);
        }
    }
}