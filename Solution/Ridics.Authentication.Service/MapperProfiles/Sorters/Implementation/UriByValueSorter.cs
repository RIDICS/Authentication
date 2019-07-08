using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Clients;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class UriByValueSorter : IUriSorter
    {
        public List<UriViewModel> SortUris(List<UriViewModel> uris)
        {
            uris = uris.OrderBy(x => x.Value).ToList();

            return uris;
        }

        public IList<UriModel> SortUris(IList<UriModel> uris)
        {
            uris = uris.OrderBy(x => x.Value).ToList();

            return uris;
        }
    }
}