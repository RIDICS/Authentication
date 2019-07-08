using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ResourcePermissionTypeActionModel : ResourcePermissionTypeActionInfoModel
    {
        public IList<RoleInfoModel> Roles { get; set; }

        public IList<UserInfoModel> Users { get; set; }
    }
}