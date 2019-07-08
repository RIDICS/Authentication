namespace Ridics.Authentication.Core.Models
{
    public class ExternalUserLoginModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ProviderKey { get; set; }

        public ExternalLoginProviderModel LoginProvider { get; set; }
    }
}