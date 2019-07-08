using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class ApiResourceByNameSorter : IApiResourceSorter
    {
        public List<ApiResourceViewModel> SortApiResources(List<ApiResourceViewModel> apiResources)
        {
            apiResources = apiResources.OrderBy(x => x.Name).ToList();

            return apiResources;
        }

        public IList<ApiResourceModel> SortApiResources(IList<ApiResourceModel> apiResources)
        {
            apiResources = apiResources.OrderBy(x => x.Name).ToList();

            return apiResources;
        }
    }
}