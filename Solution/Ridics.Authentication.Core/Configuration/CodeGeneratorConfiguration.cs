namespace Ridics.Authentication.Core.Configuration
{
    public class CodeGeneratorConfiguration
    {
        public int MaxGenerateVerificationCodeRetries { get; set; } = 10;

        public int MaxGenerateUsernameRetries { get; set; } = 10;
    }
}