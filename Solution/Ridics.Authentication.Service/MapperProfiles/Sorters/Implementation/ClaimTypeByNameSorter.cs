using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class ClaimTypeByNameSorter : IClaimTypeSorter
    {
        public List<ClaimTypeViewModel> SortClaimTypes(List<ClaimTypeViewModel> claimTypes)
        {
            claimTypes = claimTypes.OrderBy(x => x.Name).ToList();

            return claimTypes;
        }

        public List<ClaimTypeModel> SortClaimTypes(List<ClaimTypeModel> claimTypes)
        {
            claimTypes = claimTypes.OrderBy(x => x.Name).ToList();

            return claimTypes;
        }

        public IList<ClaimTypeModel> SortClaimTypes(IList<ClaimTypeModel> claimTypes)
        {
            claimTypes = claimTypes.OrderBy(x => x.Name).ToList();

            return claimTypes;
        }
    }
}