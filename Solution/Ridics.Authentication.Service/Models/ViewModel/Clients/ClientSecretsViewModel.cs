using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.Models.ViewModel.Clients
{
    public class ClientSecretsViewModel : ListViewModel<SecretViewModel>
    {
        public int ClientId { get; set; }
    }
}