using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ResourcePermissionModel : ResourcePermissionInfoModel
    {
        public IList<RoleInfoModel> Roles { get; set; }

        public IList<UserInfoModel> Users { get; set; }
    }
}