using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Models.ViewModel.Permission;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IPermissionSorter
    {
        List<PermissionViewModel> SortPermissions(List<PermissionViewModel> permissions);
        List<PermissionContractBase> SortPermissions(List<PermissionContractBase> permissions);
        IList<PermissionInfoModel> SortPermissions(IList<PermissionInfoModel> permissions);
        List<PermissionModel> SortPermissions(List<PermissionModel> permissions);
    }
}