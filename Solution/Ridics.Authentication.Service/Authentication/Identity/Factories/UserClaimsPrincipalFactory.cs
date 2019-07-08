using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Core.Shared.Types;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Authentication.Identity.Factories
{
    public class UserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly IMapper m_mapper;

        public UserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> options, IMapper mapper) : base(userManager, roleManager, options)
        {
            m_mapper = mapper;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);

            var firstNameUserData = user.UserData.First(x => x.UserDataType.DataTypeValue == UserDataTypes.FirstName);
            var lastNameUserData = user.UserData.First(x => x.UserDataType.DataTypeValue == UserDataTypes.LastName);

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Email, string.IsNullOrEmpty(user.Email) ? string.Empty : user.Email),
                new Claim(JwtClaimTypes.PhoneNumber, string.IsNullOrEmpty(user.PhoneNumber) ? string.Empty : user.PhoneNumber),
                new Claim(JwtClaimTypes.GivenName, string.IsNullOrEmpty(firstNameUserData.Value) ? string.Empty : firstNameUserData.Value),
                new Claim(JwtClaimTypes.FamilyName, string.IsNullOrEmpty(lastNameUserData.Value) ? string.Empty : lastNameUserData.Value),

                new Claim(CustomClaimTypes.EmailConfirmed, user.EmailConfirmed.ToString()),
                new Claim(CustomClaimTypes.PhoneNumberConfirmed, user.PhoneNumberConfirmed.ToString()),
                new Claim(CustomClaimTypes.MasterUserId, user.MasterUserId.ToString()),
                new Claim(CustomClaimTypes.LastUpdate, user.LastChange.ToString("O"), ClaimValueTypes.DateTime)
            };

            if (user.Permissions != null)
            {
                var permissionClaims = user.Permissions.Select(x => new Claim(CustomClaimTypes.Permission, x)).ToList();
                claims.AddRange(permissionClaims);
            }

            if (user.ResourcePermissions != null)
            {
                var permissionClaims = user.ResourcePermissions.Select(x => new Claim(CustomClaimTypes.ResourcePermission,
                    FormatPermissionClaim(x))).ToList();
                claims.AddRange(permissionClaims);
            }

            if (user.ResourcePermissionTypeActions != null)
            {
                var permissionTypesClaims = user.ResourcePermissionTypeActions.Select(x =>
                    new Claim(CustomClaimTypes.ResourcePermissionType,
                        FormatPermissionClaim(x))).ToList();
                claims.AddRange(permissionTypesClaims);
            }

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    var permissionClaims = role.ResourcePermissions.Select(x => new Claim(CustomClaimTypes.ResourcePermission,
                        FormatPermissionClaim(x))).ToList();
                    claims.AddRange(permissionClaims);

                    var permissionTypesClaims = role.ResourcePermissionTypeActions.Select(x =>
                        new Claim(CustomClaimTypes.ResourcePermissionType,
                            FormatPermissionClaim(x))).ToList();
                    claims.AddRange(permissionTypesClaims);
                }
            }

            if (user.UserClaims != null) claims.AddRange(user.UserClaims);

            claimsIdentity.AddClaims(claims);

            return claimsIdentity;
        }

        private string FormatPermissionClaim(ResourcePermissionModel resourcePermission)
        {
            return string.Format("{0}:{1}:{2}", resourcePermission.ResourceTypeAction.ResourcePermissionType.Name,
                resourcePermission.ResourceId, resourcePermission.ResourceTypeAction.Name);
        }

        private string FormatPermissionClaim(ResourcePermissionTypeActionModel resourceTypeAction)
        {
            return string.Format("{0}:{1}", resourceTypeAction.ResourcePermissionType.Name, resourceTypeAction.Name);
        }
    }
}