using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Models.ViewModel.Secrets;

namespace Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources
{
    public class ApiResourceViewModel : ResourceViewModel
    {
        [Display(Name = "api-secrets")]
        public IList<SecretViewModel> ApiSecrets { get; set; }

        [Display(Name = "scopes")]
        public IList<ScopeViewModel> Scopes { get; set; }
    }
}