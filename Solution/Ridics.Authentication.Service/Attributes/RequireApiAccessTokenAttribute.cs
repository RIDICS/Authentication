using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Service.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireApiAccessTokenAttribute : TypeFilterAttribute
    {
        public const string ApiAccessKeyHeader = "X-Api-Access-Key";

        public RequireApiAccessTokenAttribute(ApiAccessPermissionEnumModel requiredPermission = ApiAccessPermissionEnumModel.Internal)
            : base(typeof(RequireApiTokenAttributeImpl))
        {
            Arguments = new object[]
            {
                requiredPermission
            };
        }

        private class RequireApiTokenAttributeImpl : IAuthorizationFilter
        {
            private readonly ApiAccessKeyManager m_accessKeyManager;
            private readonly ApiAccessPermissionEnumModel m_requiredPermission;
            private readonly ILogger<RequireApiTokenAttributeImpl> m_logger;
            

            public RequireApiTokenAttributeImpl(
                ApiAccessKeyManager accessKeyManager,
                ApiAccessPermissionEnumModel requiredPermission,
                ILogger<RequireApiTokenAttributeImpl> logger)
            {
                m_accessKeyManager = accessKeyManager;
                m_requiredPermission = requiredPermission;
                m_logger = logger;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (
                    context.HttpContext.Request.Scheme != Uri.UriSchemeHttps
                    // This constant is used to enable environment without trusted certificate (e.g. Linux)
                    && Environment.GetEnvironmentVariable("ASPNETCORE_DISABLE_HTTPS_REDIRECT") != "true"
                )
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                if (context.HttpContext.Request.Headers.TryGetValue(ApiAccessKeyHeader, out var appTokenHeader))
                {
                    if (string.IsNullOrEmpty(appTokenHeader))
                    {
                        // missing app key
                        context.Result = new UnauthorizedResult();
                        return;
                    }

                    if (m_accessKeyManager.VerifyApplicationToken(appTokenHeader, m_requiredPermission))
                    {
                        return;
                    }
                }
                
                if (m_logger.IsEnabled(LogLevel.Information))
                {
                    m_logger.LogInformation("User attempted to access controller without application token");
                }

                context.Result = new UnauthorizedResult();
            }

           
        }
    }
}