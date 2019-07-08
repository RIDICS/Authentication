using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class IdentityResourceByNameSorter : IIdentityResourceSorter
    {
        public List<IdentityResourceViewModel> SortIdentityResources(List<IdentityResourceViewModel> identityResources)
        {
            identityResources = identityResources.OrderBy(x => x.Name).ToList();

            return identityResources;
        }

        public IList<IdentityResourceModel> SortIdentityResources(IList<IdentityResourceModel> identityResources)
        {
            identityResources = identityResources.OrderBy(x => x.Name).ToList();

            return identityResources;
        }
    }
}