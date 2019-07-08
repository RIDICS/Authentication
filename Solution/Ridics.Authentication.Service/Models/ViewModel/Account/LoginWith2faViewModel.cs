using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public class LoginWith2FaViewModel
    {
        [Required(ErrorMessage = "code-required")]
        [DataType(DataType.Text)]
        [Display(Name = "authenticator-code")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "remember-this-machine")]
        public bool RememberMachine { get; set; }

        [Display(Name = "remember-me")]
        public bool RememberMe { get; set; }

        [Display(Name = "return-url")]
        public string ReturnUrl { get; set; }

        public string Username { get; set; }

        public bool ShowResendMessage { get; set; }
    }
}