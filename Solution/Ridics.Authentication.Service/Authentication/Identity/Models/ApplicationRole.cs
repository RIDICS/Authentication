using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.Authentication.Identity.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }

        public List<PermissionModel> Permissions { get; set; }

        public List<ResourcePermissionModel> ResourcePermissions { get; set; }

        public List<ResourcePermissionTypeActionModel> ResourcePermissionTypeActions { get; set; }
    }
}