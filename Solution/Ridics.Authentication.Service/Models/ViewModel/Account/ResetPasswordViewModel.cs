using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Configuration;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public class ResetPasswordViewModel 
    {
        [Required(ErrorMessage = "username-required")]
        [Display(Name = "username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "token-required")]
        public string PasswordResetToken { get; set; }

        [Required(ErrorMessage = "pswd-required"), DataType(DataType.Password)]
        [Display(Name = "pswd")]
        [RegularExpression(PasswordRequirements.ValidationRegexOfViewModel, ErrorMessage = PasswordRequirements.ErrorOfResetPasswordViewModel)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "pswd-match")]
        [Display(Name = "pswd-repeat")]
        public string ConfirmPassword { get; set; }

        public bool IsVerified { get; set; }
    }
}