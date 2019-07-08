using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ResourcePermissionTypeModel : ResourcePermissionTypeInfoModel
    {
        public IList<ResourcePermissionTypeActionInfoModel> ResourcePermissionTypeActions { get; set; }
    }
}