namespace Ridics.Authentication.Service.Models.ViewModel.Secrets
{
    public class EditableSecretViewModel : SecretViewModel
    {
        public int ApiResourceId { get; set; }

        public int ClientId { get; set; }
    }
}