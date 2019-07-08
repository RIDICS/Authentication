using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Service.Authentication.Identity.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Roles;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class RoleByNameSorter : IRoleSorter
    {
        public List<RoleViewModel> SortRoles(List<RoleViewModel> roles)
        {
            roles = roles.OrderBy(x => x.Name).ToList();

            return roles;
        }
       
        public List<RoleContract> SortRoles(List<RoleContract> roles)
        {
            roles = roles.OrderBy(x => x.Name).ToList();

            return roles;
        }

        public IList<RoleInfoModel> SortRoles(IList<RoleInfoModel> roles)
        {
            roles = roles.OrderBy(x => x.Name).ToList();

            return roles;
        }

        public IList<RoleModel> SortRoles(IList<RoleModel> roles)
        {
            roles = roles.OrderBy(x => x.Name).ToList();

            return roles;
        }

        public List<ApplicationRole> SortRoles(List<ApplicationRole> roles)
        {
            roles = roles.OrderBy(x => x.Name).ToList();

            return roles;
        }
    }
}