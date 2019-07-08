using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Keys
{
    public class ApiAccessKeyHashViewModel{
        [Display(Name = "name")]
        public string Name { get; set; }

        [Display(Name = "algorithm")]
        [Required(ErrorMessage = "algorithm-required")]
        public string Algorithm { get; set; }
    }
}