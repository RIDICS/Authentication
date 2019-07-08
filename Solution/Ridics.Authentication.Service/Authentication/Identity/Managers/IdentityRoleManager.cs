using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Service.Authentication.Identity.Models;

namespace Ridics.Authentication.Service.Authentication.Identity.Managers
{
    public class IdentityRoleManager : RoleManager<ApplicationRole>
    {
        public IdentityRoleManager(IRoleStore<ApplicationRole> store, IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<ApplicationRole>> logger) : base(store,
            roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}