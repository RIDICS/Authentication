using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources
{
    public class ApiResourceSecretsViewModel : ListViewModel<SecretViewModel>
    {
        public int ApiResourceId { get; set; }
    }
}