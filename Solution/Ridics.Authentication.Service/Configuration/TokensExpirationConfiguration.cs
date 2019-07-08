namespace Ridics.Authentication.Service.Configuration
{
    public class TokensExpirationConfiguration
    {
        public int TwoFactorTokenExpirationInSeconds { get; set; }

        public int PasswordResetTokenExpirationInSeconds { get; set; }
    }
}
