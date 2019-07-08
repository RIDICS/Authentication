using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IIdentityResourceSorter
    {
        List<IdentityResourceViewModel> SortIdentityResources(List<IdentityResourceViewModel> identityResources);
        IList<IdentityResourceModel> SortIdentityResources(IList<IdentityResourceModel> identityResources);
    }
}