using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IScopeSorter
    {
        IList<ScopeViewModel> SortScopes(IList<ScopeViewModel> scopes);
        IList<ScopeModel> SortScopes(IList<ScopeModel> scopes);
    }
}