using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class ResourcePermissionByResourceIdSorter : IResourcePermissionSorter
    {
        public IList<ResourcePermissionInfoModel> SortResourcePermissions(IList<ResourcePermissionInfoModel> resourcePermissions)
        {
            resourcePermissions = resourcePermissions.OrderBy(x => x.ResourceId).ToList();

            return resourcePermissions;
        }

        public List<ResourcePermissionModel> SortResourcePermissions(List<ResourcePermissionModel> resourcePermissions)
        {
            resourcePermissions = resourcePermissions.OrderBy(x => x.ResourceId).ToList();

            return resourcePermissions;
        }

        public List<ResourcePermissionViewModel> SortResourcePermissions(List<ResourcePermissionViewModel> resourcePermissions)
        {
            resourcePermissions = resourcePermissions.OrderBy(x => x.ResourceId).ToList();

            return resourcePermissions;
        }
    }
}