namespace Ridics.Authentication.Service.Configuration
{
    // Maybe find better (more dynamically configurable) solution for password requirements
    public class PasswordRequirements
    {
        // When updating these values, it is also required to update password requirements specified on client app

        public const int MinLength = 8;
        public const bool RequireDigit = true;
        public const bool RequireLowercase = true;
        public const bool RequireUppercase = true;
        public const bool RequireNonAlphanumeric = false;

        public const string ValidationRegexOfViewModel = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,}$";
        public const string ErrorOfUserWithPasswordViewModel = "pswd-requirement";
        public const string ErrorOfResetPasswordViewModel = "pswd-requirement";
    }
}
