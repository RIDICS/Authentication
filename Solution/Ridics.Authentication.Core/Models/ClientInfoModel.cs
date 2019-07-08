namespace Ridics.Authentication.Core.Models
{
    public class ClientInfoModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DisplayUrl { get; set; }

        public string LogoUrl { get; set; }

        public bool RequireConsent { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; }
    }
}