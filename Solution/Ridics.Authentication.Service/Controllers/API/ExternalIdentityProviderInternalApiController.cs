using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiVersion("1.0")]
    [Route(ApiRouteStart + "/v{version:apiVersion}/externalLoginProvider")]
    [JwtAuthorize(Policy = PolicyNames.InternalApiPolicy)]
    public class ExternalIdentityProviderInternalApiV1Controller : ApiControllerBase
    {
        private readonly ExternalLoginProviderManager m_externalLoginProviderManager;
        private readonly ITranslator m_translator;

        public ExternalIdentityProviderInternalApiV1Controller(
            ExternalLoginProviderManager externalLoginProviderManager,
            IdentityUserManager identityUserManager, ITranslator translator) : base(identityUserManager)
        {
            m_externalLoginProviderManager = externalLoginProviderManager;
            m_translator = translator;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(ListContract<ExternalLoginProviderContract>), StatusCodes.Status200OK)]
        public IActionResult ListExternalIdentityProvider([FromQuery] int start = 0, [FromQuery] int count = DefaultListCount)
        {
            if (count > MaxListCount)
            {
                count = MaxListCount;
            }

            var externalIdentityProvidersResult = m_externalLoginProviderManager.FindExternalLoginProviders(start, count);

            var externalIdentityProviderCount = m_externalLoginProviderManager.GetExternalLoginProvidersCount();

            if (externalIdentityProvidersResult.HasError)
            {
                return Error(externalIdentityProvidersResult.Error);
            }

            if (externalIdentityProviderCount.HasError)
            {
                return Error(externalIdentityProviderCount.Error);
            }

            var externalIdentityProviderContracts = Mapper.Map<IList<ExternalLoginProviderContract>>(
                externalIdentityProvidersResult.Result
            );

            TranslateExternalIdentityProviderDescription(externalIdentityProviderContracts);

            var contractList = new ListContract<ExternalLoginProviderContract>
            {
                Items = externalIdentityProviderContracts,
                ItemsCount = externalIdentityProviderCount.Result,
            };

            Response.Headers.Add(HeaderStart, start.ToString());
            Response.Headers.Add(HeaderCount, count.ToString());

            return Json(contractList);
        }

        private void TranslateExternalIdentityProviderDescription(IEnumerable<ExternalLoginProviderContract> externalLoginProviderContracts)
        {
            //Translate description with culture from request
            foreach (var externalIdentityProviderContract in externalLoginProviderContracts)
            {
                externalIdentityProviderContract.Description =
                    m_translator.Translate($"{externalIdentityProviderContract.DescriptionLocalizationKey}", "external-identity");
            }
        }
    }
}