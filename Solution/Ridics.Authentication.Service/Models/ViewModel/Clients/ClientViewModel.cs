using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Helpers;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.Models.ViewModel.Clients
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        [Display(Name = "name")]
        [Required(ErrorMessage = "name-required")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "secrets")]
        public List<SecretViewModel> Secrets { get; set; }

        [Display(Name = "grant-types")]
        public List<GrantTypeViewModel> AllowedGrantTypes { get; set; }

        [Display(Name = "uris")]
        public List<UriViewModel> UriList { get; set; }

        [Display(Name = "display-url")]
        [CustomUri(ErrorMessage = "not-valid-uri")]
        public string DisplayUrl { get; set; }

        [Display(Name = "logo-url")]
        [CustomUri(ErrorMessage = "not-valid-uri")]
        public string LogoUrl { get; set; }

        [Display(Name = "require-consent")]
        [UIHint("_BoolToStringTemplate")]
        public bool RequireConsent { get; set; }

        [Display(Name = "identity-resources")]
        public List<IdentityResourceViewModel> AllowedIdentityResources { get; set; }

        [Display(Name = "allow-access-tokens-via-browser")]
        [UIHint("_BoolToStringTemplate")]
        public bool AllowAccessTokensViaBrowser { get; set; }

        [Display(Name = "scopes")]
        public List<ScopeViewModel> AllowedScopes { get; set; }
    }
}