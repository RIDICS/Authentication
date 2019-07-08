using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.Models.ViewModel.Users.Claims
{
    public class ClaimViewModel
    {
        [DisplayName("type")]
        public ClaimTypeViewModel Type { get; set; }

        [DisplayName("value")]
        [Required(ErrorMessage = "value-required")]
        public string Value { get; set; }
    }
}