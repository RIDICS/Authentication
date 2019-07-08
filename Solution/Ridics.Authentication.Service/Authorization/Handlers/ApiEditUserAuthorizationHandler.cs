using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Ridics.Authentication.Service.Authorization.Requirements;
using Ridics.Core.Structures;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Authorization.Handlers
{
    public class ApiEditUserAuthorizationHandler : AuthorizationHandler<ApiEditUserRequirement>
    {
        /// <summary>
        /// Handles <see cref="ApiEditUserRequirement" />.     
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns>Completed task</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiEditUserRequirement requirement)
        {
            var user = context.User;

            if (user == null) return Task.CompletedTask;

            // TODO use common class for getting user ID instead of direct access to claims, ApiControllerBase currently contains method GetUserId() for getting userId:
            //By default asp.net maps id of user from sub field inside JWT to name claim
            var loggedUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (loggedUserIdClaim == null) return Task.CompletedTask;

            if (!int.TryParse(loggedUserIdClaim.Value, out var loggedUserId)) return Task.CompletedTask;

            if (loggedUserId == requirement.UserId)
            {
                if (!requirement.NeedPermissionToEditSelf)
                {
                    context.Succeed(requirement);
                    
                }
                else if (requirement.PermissionRequiredToEditSelf == null)
                {
                    throw new ArgumentException("Required permission is need to be specified for this action when editing self");
                }
                else if (user.HasClaim(CustomClaimTypes.Permission, requirement.PermissionRequiredToEditSelf))
                {
                    context.Succeed(requirement);
                }

                return Task.CompletedTask;
            }

            if (!requirement.CanOtherUserEdit)
            {
                return Task.CompletedTask;
            }

            if (requirement.PermissionRequiredToEditOtherUser == null)
            {
                throw new ArgumentException("Required permission is need to be specified for editing other users");
            }

            if (user.HasClaim(CustomClaimTypes.Permission, requirement.PermissionRequiredToEditOtherUser))
            {
                context.Succeed(requirement);
            }
            
            return Task.CompletedTask;
        }
    }
}