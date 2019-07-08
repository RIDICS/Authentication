using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Ridics.Authentication.Service.Authorization.Requirements;
using Ridics.Core.Structures;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Authorization
{
    public static class AuthorizationExtensions
    {
        public static async Task<bool> AuthorizationForUserEditAsync(this IAuthorizationService service, ClaimsPrincipal claimsPrincipal, string role, int id)
        {
            var result = await service.AuthorizeAsync(claimsPrincipal, null, new EditUserRequirement(role, id));

            return result.Succeeded;
        }

        public static async Task<bool> ApiAuthorizationForUserEditAsync(this IAuthorizationService service, ClaimsPrincipal claimsPrincipal, 
            int id, bool needPermissionToEditSelf, bool canOtherUserEdit, string permissionRequiredToEditSelf = null,
            string permissionRequiredToEditOtherUser = null)
        {
            var result = await service.AuthorizeAsync(claimsPrincipal, null, new ApiEditUserRequirement
            {
                UserId = id,
                CanOtherUserEdit = canOtherUserEdit,
                NeedPermissionToEditSelf = needPermissionToEditSelf,
                PermissionRequiredToEditOtherUser = permissionRequiredToEditOtherUser,
                PermissionRequiredToEditSelf = permissionRequiredToEditSelf
            });

            return result.Succeeded;
        }

        public static bool HasPermission(this ClaimsPrincipal claimsPrincipal, string permissionName)
        {
            return claimsPrincipal.Claims.Where(x => x.Type == CustomClaimTypes.Permission)
                .Any(y => y.Value == permissionName);
        }

        public static bool HasAnyPermission(this ClaimsPrincipal claimsPrincipal, params string[] permissionName)
        {
            return permissionName.Any(x => HasPermission(claimsPrincipal, x));
        }

    }
}