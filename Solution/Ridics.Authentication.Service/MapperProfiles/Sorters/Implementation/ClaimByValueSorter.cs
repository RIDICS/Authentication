using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class ClaimByValueSorter : IClaimSorter
    {
        public IList<ClaimModel> SortClaims(IList<ClaimModel> claims)
        {
            claims = claims.OrderBy(x => x.Value).ToList();

            return claims;
        }
    }
}