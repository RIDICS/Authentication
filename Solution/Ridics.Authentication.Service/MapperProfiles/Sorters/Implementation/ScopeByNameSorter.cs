using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class ScopeByNameSorter : IScopeSorter
    {
        public IList<ScopeViewModel> SortScopes(IList<ScopeViewModel> scopes)
        {
            scopes = scopes.OrderBy(x => x.Name).ToList();

            return scopes;
        }

        public IList<ScopeModel> SortScopes(IList<ScopeModel> scopes)
        {
            scopes = scopes.OrderBy(x => x.Name).ToList();

            return scopes;
        }
    }
}