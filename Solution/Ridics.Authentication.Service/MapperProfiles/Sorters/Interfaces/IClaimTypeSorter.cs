using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IClaimTypeSorter
    {
        List<ClaimTypeViewModel> SortClaimTypes(List<ClaimTypeViewModel> claimTypes);
        List<ClaimTypeModel> SortClaimTypes(List<ClaimTypeModel> claimTypes);
        IList<ClaimTypeModel> SortClaimTypes(IList<ClaimTypeModel> claimTypes);
    }
}