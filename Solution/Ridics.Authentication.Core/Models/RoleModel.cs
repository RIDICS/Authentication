using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class RoleModel : RoleInfoModel
    {
        public IList<PermissionInfoModel> Permissions { get; set; }

        public IList<ResourcePermissionInfoModel> ResourcePermissions { get; set; }

        public IList<ResourcePermissionTypeActionInfoModel> ResourcePermissionTypeActions { get; set; }
    }
}