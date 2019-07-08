using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Clients;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IUriSorter
    {
        List<UriViewModel> SortUris(List<UriViewModel> uris);
        IList<UriModel> SortUris(IList<UriModel> uris);
    }
}