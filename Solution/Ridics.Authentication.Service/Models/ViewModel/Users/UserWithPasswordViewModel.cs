using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Configuration;

namespace Ridics.Authentication.Service.Models.ViewModel.Users
{
    public class UserWithPasswordViewModel : UserViewModel
    {
        [Display(Name = "pswd")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "pswd-required")]
        [RegularExpression(PasswordRequirements.ValidationRegexOfViewModel, ErrorMessage = PasswordRequirements.ErrorOfUserWithPasswordViewModel)]
        public string Password { get; set; }

        [Display(Name = "pswd-again")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "pswds-mismatch")]
        public string ConfirmPassword { get; set; }
    }
}