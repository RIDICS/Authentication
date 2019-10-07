using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.Models.ViewModel.Roles;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IRoleSorter
    {
        List<RoleViewModel> SortRoles(List<RoleViewModel> roles);
        List<RoleContractBase> SortRoles(List<RoleContractBase> roles);
        IList<RoleInfoModel> SortRoles(IList<RoleInfoModel> roles);
        IList<RoleModel> SortRoles(IList<RoleModel> roles);
        List<ApplicationRole> SortRoles(List<ApplicationRole> roles);
    }
}