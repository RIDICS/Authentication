using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IApiResourceSorter
    {
        List<ApiResourceViewModel> SortApiResources(List<ApiResourceViewModel> apiResources);
        IList<ApiResourceModel> SortApiResources(IList<ApiResourceModel> apiResources);
    }
}