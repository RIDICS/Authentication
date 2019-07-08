using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface IApiSecretSorter
    {
        IList<SecretViewModel> SortApiSecrets(IList<SecretViewModel> apiSecrets);
        IList<SecretModel> SortApiSecrets(IList<SecretModel> apiSecrets);
    }
}