using System.Collections.Generic;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IClaimSorter
    {
        IList<ClaimModel> SortClaims(IList<ClaimModel> claims);
    }
}