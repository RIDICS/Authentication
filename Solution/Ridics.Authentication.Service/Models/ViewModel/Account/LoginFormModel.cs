using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public partial class LoginViewModel
    {
        public class LoginFormModel
        {
            [Required(ErrorMessage = "username-required")]
            [Display(Name = "username-or-email")]
            public string Username { get; set; }

            [Required(ErrorMessage = "pswd-required")]
            [Display(Name = "pswd")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "remember-me")] 
            public bool RememberLogin { get; set; }
        }
    }
}