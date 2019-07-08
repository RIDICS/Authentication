using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public class RequestResetPasswordViewModel
    {
        [Display(Name = "return-url")]
        public string ReturnUrl { get; set; }

        [Required(ErrorMessage = "username-required")]
        [Display(Name = "usernameOrEmail")]
        public string Username { get; set; }
    }
}