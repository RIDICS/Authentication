using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class ApiSecretByValueSorter : IApiSecretSorter
    {
        public IList<SecretViewModel> SortApiSecrets(IList<SecretViewModel> apiSecrets)
        {
            apiSecrets = apiSecrets.OrderBy(x => x.Value).ToList();

            return apiSecrets;
        }

        public IList<SecretModel> SortApiSecrets(IList<SecretModel> apiSecrets)
        {
            apiSecrets = apiSecrets.OrderBy(x => x.Value).ToList();

            return apiSecrets;
        }
    }
}