using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public class ResetPasswordViewModel 
    {
        [Required(ErrorMessage = "username-required")]
        [Display(Name = "username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "token-required")]
        public string PasswordResetToken { get; set; }

        //TODO extract password and its regex rules to standalone viewmodel and fix all usages
        [Required(ErrorMessage = "pswd-required"), DataType(DataType.Password)]
        [Display(Name = "pswd")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[\W_]).{8,}$", ErrorMessage = "pswd-requirement")]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "pswd-match")]
        [Display(Name = "pswd-repeat")]
        public string ConfirmPassword { get; set; }

        public bool IsVerified { get; set; }
    }
}