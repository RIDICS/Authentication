using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Core.Structures;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Authentication.Events
{
    [Obsolete("This class is obsolete, it was used for ValidatePrincipal to check if user claims are not outdated. This behavior is now part of asp Identity", true)]
    public class ValidateCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        private readonly IdentitySignInManager m_signInManager;
        private readonly ILogger<ValidateCookieAuthenticationEvents> m_logger;
        private readonly UserManager m_userManager;

        public ValidateCookieAuthenticationEvents(UserManager userManager, IdentitySignInManager signInManager,
            ILogger<ValidateCookieAuthenticationEvents> logger)
        {
            m_userManager = userManager;
            m_signInManager = signInManager;
            m_logger = logger;
        }

        /// <summary>
        /// Validates if logged-in user has up-to-date info, if not user is singed out.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var userPrincipal = context.Principal;

            var lastChanged = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == CustomClaimTypes.LastUpdate);
            var id = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.Subject);

            if (lastChanged == null || id == null)
            {
                context.RejectPrincipal();
                await m_signInManager.SignOutAsync();

                return;
            }

            var result = m_userManager.ValidateLastChanged(int.Parse(id.Value), lastChanged.Value);

            if (result.HasError)
            {
                if (m_logger.IsEnabled(LogLevel.Error))
                {
                    m_logger.LogError(string.Format("Code: {0}, Message: {1}", result.Error.Code, result.Error.Message));
                }
            }

            if (result.Result)
            {
                return;
            }

            context.RejectPrincipal();
            await m_signInManager.SignOutAsync();
        }
    }
}