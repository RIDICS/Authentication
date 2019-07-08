using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class ResourcePermissionTypeActionByNameSorter : IResourcePermissionTypeActionSorter
    {
        public IList<ResourcePermissionTypeActionViewModel> SortResourcePermissionTypeActions(IList<ResourcePermissionTypeActionViewModel> resourcePermissionTypeActions)
        {
            resourcePermissionTypeActions = resourcePermissionTypeActions.OrderBy(x => x.Name).ToList();

            return resourcePermissionTypeActions;
        }

        public List<ResourcePermissionTypeActionViewModel> SortResourcePermissionTypeActions(List<ResourcePermissionTypeActionViewModel> resourcePermissionTypeActions)
        {
            resourcePermissionTypeActions = resourcePermissionTypeActions.OrderBy(x => x.Name).ToList();

            return resourcePermissionTypeActions;
        }

        public IList<ResourcePermissionTypeActionInfoModel> SortResourcePermissionTypeActions(IList<ResourcePermissionTypeActionInfoModel> resourcePermissionTypeActions)
        {
            resourcePermissionTypeActions = resourcePermissionTypeActions.OrderBy(x => x.Name).ToList();

            return resourcePermissionTypeActions;
        }

        public List<ResourcePermissionTypeActionModel> SortResourcePermissionTypeActions(List<ResourcePermissionTypeActionModel> resourcePermissionTypeActions)
        {
            resourcePermissionTypeActions = resourcePermissionTypeActions.OrderBy(x => x.Name).ToList();

            return resourcePermissionTypeActions;
        }
    }
}