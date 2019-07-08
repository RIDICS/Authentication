using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IResourcePermissionTypeActionSorter
    {
        IList<ResourcePermissionTypeActionViewModel> SortResourcePermissionTypeActions(IList<ResourcePermissionTypeActionViewModel> resourcePermissionTypeActions);
        List<ResourcePermissionTypeActionViewModel> SortResourcePermissionTypeActions(List<ResourcePermissionTypeActionViewModel> resourcePermissionTypeActions);
        IList<ResourcePermissionTypeActionInfoModel> SortResourcePermissionTypeActions(IList<ResourcePermissionTypeActionInfoModel> resourcePermissionTypeActions);
        List<ResourcePermissionTypeActionModel> SortResourcePermissionTypeActions(List<ResourcePermissionTypeActionModel> resourcePermissionTypeActions);
    }
}