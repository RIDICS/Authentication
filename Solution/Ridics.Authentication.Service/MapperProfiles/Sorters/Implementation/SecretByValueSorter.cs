using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Implementation
{
    public class SecretByValueSorter : ISecretSorter
    {
        public List<SecretViewModel> SortSecrets(List<SecretViewModel> secrets)
        {
            secrets = secrets.OrderBy(x => x.Value).ToList();

            return secrets;
        }

        public IList<SecretModel> SortSecrets(IList<SecretModel> secrets)
        {
            secrets = secrets.OrderBy(x => x.Value).ToList();

            return secrets;
        }
    }
}