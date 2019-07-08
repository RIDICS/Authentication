using System.Diagnostics;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Authorization;
using Ridics.Authentication.Service.Controllers.API;
using Ridics.Authentication.Service.Models;
using Ridics.Authentication.Service.Models.Constants;
using Scalesoft.Localization.AspNetCore;
using Ridics.Core.Structures;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorController : Controller, IErrorHandler
    {
        private readonly ILocalizationService m_localization;
        private readonly IIdentityServerInteractionService m_interaction;
        private readonly IAuthorizationService m_authorizationService;
        private readonly ReturnUrlConfiguration m_returnUrlConfiguration;

        public ErrorController(ILocalizationService localization, IIdentityServerInteractionService interaction, IAuthorizationService authorizationService, ReturnUrlConfiguration returnUrlConfiguration)
        {
            m_localization = localization;
            m_interaction = interaction;
            m_authorizationService = authorizationService;
            m_returnUrlConfiguration = returnUrlConfiguration;
        }

        [Route("Error")]
        [Route("Error/{errorCode}")]
        public async Task<IActionResult> Index(string errorCode, [FromQuery] string errorId = null)
        {
            var errorMessage = m_localization.Translate("unknown-error-msg", "error");
            var errorMessageDetail = string.Empty;

            if (!string.IsNullOrEmpty(errorId))
            {
                var message = await m_interaction.GetErrorContextAsync(errorId);
                
                errorMessageDetail = message.ErrorDescription;
            }
            else if (int.TryParse(errorCode, out var errorCodeNumber))
            {
                if (errorCodeNumber == StatusCodes.Status500InternalServerError)
                {
                    ViewData[ViewDataKeys.IsError] = true;
                }

                var apiErrorCode = DataResultErrorCode.GenericError;
                switch (errorCodeNumber)
                {
                    case StatusCodes.Status400BadRequest:
                        errorMessage = m_localization.Translate("bad-request-msg", "error");
                        errorMessageDetail = m_localization.Translate("bad-request-detail", "error");
                        apiErrorCode = DataResultErrorCode.BadRequest400;
                        break;
                    case StatusCodes.Status401Unauthorized:
                        errorMessage = m_localization.Translate("unauthorized-msg", "error");
                        errorMessageDetail = m_localization.Translate("unauthorized-detail", "error");
                        apiErrorCode = DataResultErrorCode.Unauthorized401;
                        break;
                    case StatusCodes.Status403Forbidden:
                        errorMessage = m_localization.Translate("forbidden-msg", "error");
                        errorMessageDetail = m_localization.Translate("forbidden-detail", "error");
                        apiErrorCode = DataResultErrorCode.Forbidden403;
                        break;
                    case StatusCodes.Status404NotFound:
                        errorMessage = m_localization.Translate("not-found-msg", "error");
                        errorMessageDetail = m_localization.Translate("not-found-detail", "error");
                        apiErrorCode = DataResultErrorCode.NotFound404;
                        break;
                    case StatusCodes.Status500InternalServerError:
                        errorMessage = m_localization.Translate("internal-server-error-msg", "error");
                        errorMessageDetail = m_localization.Translate("internal-server-error-detail", "error");
                        apiErrorCode = DataResultErrorCode.InternalServerError500;
                        break;
                    case StatusCodes.Status502BadGateway:
                        errorMessage = m_localization.Translate("bad-gateway-msg", "error");
                        errorMessageDetail = m_localization.Translate("bad-gateway-detail", "error");
                        apiErrorCode = DataResultErrorCode.BadGateway502;
                        break;
                    //case StatusCodes.Status503ServiceUnavailable:
                    //    errorMessage = "";
                    //    errorMessageDetail = "";
                    //    break;
                    case StatusCodes.Status504GatewayTimeout:
                        errorMessage = m_localization.Translate("gateway-timeout-msg", "error");
                        errorMessageDetail = m_localization.Translate("gateway-timeout-detail", "error");
                        apiErrorCode = DataResultErrorCode.GatewayTimeout504;
                        break;
                }

                //If error happened on API return JSON
                if (IsApiRequest()) 
                {
                    return Json(new ContractException { Code = apiErrorCode, Description = errorMessage });
                }
            }

            var returnUrl = await GetReturnUrlAsync();
            
            return View("Error", new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                ErrorMessageDetail = errorMessageDetail,
                ReturnUrl = returnUrl,
            });
        }

        public IActionResult GetErrorView(int statusCode, string message, string messageDetail)
        {
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                ViewData[ViewDataKeys.IsError] = true;
            }

            var returnUrl = GetReturnUrlAsync().GetAwaiter().GetResult();

            var result = View("Error", new ErrorViewModel
            {
                RequestId = null,
                ErrorCode = statusCode.ToString(),
                ErrorMessage = message,
                ErrorMessageDetail = messageDetail,
                ReturnUrl = returnUrl,
            });
            result.StatusCode = statusCode;

            return result;
        }

        public IActionResult GetErrorView(int statusCode)
        {
            return Index(statusCode.ToString()).GetAwaiter().GetResult();
        }

        private async Task<string> GetReturnUrlAsync()
        {
            var authResult = await m_authorizationService.AuthorizeAsync(User, null, PolicyNames.ViewAuthServiceAdministrationPolicy);

            return authResult.Succeeded
                ? Url.Action("Index", "Home")
                : m_returnUrlConfiguration.DefaultRedirectUrl;
        }

        private bool IsApiRequest()
        {
            var origRequestPath = GetOriginalRequestPath();

            return !string.IsNullOrEmpty(origRequestPath) && origRequestPath.StartsWithSegments(ApiControllerBase.ApiRouteStart);
        }

        private PathString GetOriginalRequestPath()
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var origRequestPath = feature?.OriginalPath;

            return new PathString(origRequestPath);
        }
    }
}
