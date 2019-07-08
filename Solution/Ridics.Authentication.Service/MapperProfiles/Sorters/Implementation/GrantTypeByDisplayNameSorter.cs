using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Clients;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class GrantTypeByDisplayNameSorter : IGrantTypeSorter
    {
        public List<GrantTypeViewModel> SortGrantTypes(List<GrantTypeViewModel> grantTypes)
        {
            grantTypes = grantTypes.OrderBy(x => x.DisplayName).ToList();

            return grantTypes;
        }

        public IList<GrantTypeModel> SortGrantTypes(IList<GrantTypeModel> grantTypes)
        {
            grantTypes = grantTypes.OrderBy(x => x.DisplayName).ToList();

            return grantTypes;
        }
    }
}