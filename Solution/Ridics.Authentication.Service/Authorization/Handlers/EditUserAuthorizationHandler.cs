using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Ridics.Authentication.Service.Authentication.Identity.Managers;
using Ridics.Authentication.Service.Authorization.Requirements;

namespace Ridics.Authentication.Service.Authorization.Handlers
{
    public class EditUserAuthorizationHandler : AuthorizationHandler<EditUserRequirement>
    {
        private readonly IdentityUserManager m_userManager;

        public EditUserAuthorizationHandler(IdentityUserManager userManager)
        {
            m_userManager = userManager;
        }

        /// <summary>
        /// Checks if logged user has permission to edit user. (is in appropriate role or wants to edit himself)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement">Stores appropriate role or id of edited user</param>
        /// <returns></returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EditUserRequirement requirement)
        {
            //var isInRole = context.User.IsInRole(requirement.Role);

            var userIdClaim = context.User?.FindFirst(JwtClaimTypes.Subject);

            var userIdParseSucceeded = int.TryParse(userIdClaim?.Value, out var userId);

            if (userIdParseSucceeded)
            {
                var isInRole = m_userManager.IsInRoleAsync(userId, requirement.Role).Result;

                if (isInRole)
                {
                    context.Succeed(requirement);
                }

                if (userId == requirement.UserId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}