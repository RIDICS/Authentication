using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Clients;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IGrantTypeSorter
    {
        List<GrantTypeViewModel> SortGrantTypes(List<GrantTypeViewModel> grantTypes);
        IList<GrantTypeModel> SortGrantTypes(IList<GrantTypeModel> grantTypes);
    }
}