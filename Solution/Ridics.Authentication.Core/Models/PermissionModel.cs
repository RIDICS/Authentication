using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class PermissionModel : PermissionInfoModel
    {
        public IList<RoleInfoModel> Roles { get; set; }
    }
}