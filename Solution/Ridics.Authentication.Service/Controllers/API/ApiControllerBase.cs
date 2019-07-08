using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Utils;

namespace Ridics.Authentication.Service.Controllers.API
{
    [ApiController]
    [IgnoreAntiforgeryToken]
    [ApiExceptionFilter]
    [LocalizeApiByHeaderFilter]
    public abstract class ApiControllerBase : Controller
    {
        public const string ApiRouteStart = "/api";
        public const string HeaderCulture = "X-Culture";

        protected const string HeaderStart = "X-Start";
        protected const string HeaderCount = "X-Count";
        protected const string HeaderFileName = "X-FileName";
        protected const string HeaderGuid = "X-Guid";
        protected const string HeaderType = "X-Type";
        protected const string HeaderFileExtension = "X-FileExtension";

        protected const int DefaultListCount = 20;
        protected const int MaxListCount = 50;

        protected readonly IdentityUserManager m_identityUserManager;

        protected ApiControllerBase(IdentityUserManager identityUserManager)
        {
            m_identityUserManager = identityUserManager;
        }

        protected string GetMethodCalledLoggingString(string parameters, object data, [CallerMemberName] string methodName = null)
        {
            var userId = GetUserId();
            var serializedDataContract = JsonConvert.SerializeObject(data);
            return
                $"{methodName} called, time: {DateTime.UtcNow}, user: {User.Identity.Name}, userId: {userId}, parameters: {parameters}, dataContract={serializedDataContract}";
        }

        protected string GetUserId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return null;
            }

            var userId = m_identityUserManager.GetUserId(User);

            if (userId == null)
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return userId;
        }

        protected ActionResult Error(string message = null, string code = null)
        {
            if (string.IsNullOrEmpty(message) && string.IsNullOrEmpty(code))
            {
                return BadRequest();
            }

            return BadRequest(new ContractException
            {
                Code = string.IsNullOrEmpty(code) ? ContractExceptionCode.GenericError : code,
                Description = message,
            });
        }

        protected ActionResult Error(DataResultError resultError)
        {
            return Error(resultError.Message, resultError.Code);
        }
    }
}