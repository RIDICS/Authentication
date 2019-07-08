using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Models.ViewModel.Clients;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public class ConsentViewModel
    {
        public ClientViewModel Client { get; set; }

        public string ReturnUrl { get; set; }

        public List<SelectableViewModel<IdentityResourceViewModel>> IdentityResources { get; set; }

        public List<SelectableViewModel<ScopeViewModel>> Scopes { get; set; }

        [Display(Name = "remember-consent")]
        public bool RememberConsent { get; set; }
    }
}