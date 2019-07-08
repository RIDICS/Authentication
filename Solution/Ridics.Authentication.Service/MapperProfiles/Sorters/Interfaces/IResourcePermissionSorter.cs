using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IResourcePermissionSorter
    {
        IList<ResourcePermissionInfoModel> SortResourcePermissions(IList<ResourcePermissionInfoModel> resourcePermissions);
        List<ResourcePermissionModel> SortResourcePermissions(List<ResourcePermissionModel> resourcePermissions);
        List<ResourcePermissionViewModel> SortResourcePermissions(List<ResourcePermissionViewModel> resourcePermissions);
    }
}