using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class PermissionByNameSorter : IPermissionSorter
    {
        public List<PermissionViewModel> SortPermissions(List<PermissionViewModel> permissions)
        {
            permissions = permissions.OrderBy(x => x.Name).ToList();

            return permissions;
        }

        public List<PermissionContractBase> SortPermissions(List<PermissionContractBase> permissions)
        {
            permissions = permissions.OrderBy(x => x.Name).ToList();

            return permissions;
        }

        public IList<PermissionInfoModel> SortPermissions(IList<PermissionInfoModel> permissions)
        {
            permissions = permissions.OrderBy(x => x.Name).ToList();

            return permissions;
        }

        public List<PermissionModel> SortPermissions(List<PermissionModel> permissions)
        {
            permissions = permissions.OrderBy(x => x.Name).ToList();

            return permissions;
        }
    }
}