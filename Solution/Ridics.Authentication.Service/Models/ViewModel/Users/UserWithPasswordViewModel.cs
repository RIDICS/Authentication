using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.Users
{
    public class UserWithPasswordViewModel : UserViewModel
    {
        //TODO extract password and its regex rules to standalone viewmodel and fix all usages
        [Display(Name = "pswd")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "pswd-required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[\W_]).{8,}$", ErrorMessage = "pswd-requirement")]
        public string Password { get; set; }

        [Display(Name = "pswd-again")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "pswds-mismatch")]
        public string ConfirmPassword { get; set; }
    }
}