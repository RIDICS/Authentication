using System.Collections.Generic;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.MapperProfiles.Sorters.Interfaces
{
    public interface ISecretSorter
    {
        List<SecretViewModel> SortSecrets(List<SecretViewModel> secrets);
        IList<SecretModel> SortSecrets(IList<SecretModel> secrets);
    }
}