using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources
{
    public class ScopeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "name")]
        [Required(ErrorMessage = "name-required")]
        public string Name { get; set; }

        [Display(Name = "description")]
        public string Description { get; set; }

        [Display(Name = "claims")]
        public List<ClaimTypeViewModel> Claims { get; set; }

        [Display(Name = "required")]
        [UIHint("_BoolToStringTemplate")]
        public bool Required { get; set; }

        [Display(Name = "show-in-discovery-document")]
        [UIHint("_BoolToStringTemplate")]
        public bool ShowInDiscoveryDocument { get; set; }
    }
}