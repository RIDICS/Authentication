namespace Ridics.Authentication.Core.Utils.CodeGenerator.Configuration
{
    public class VerificationCodeConfiguration : CodeGeneratorConfigurationBase
    {
        public override int CodeLength { get; set; } = 8;

        public override AllowedCharactersBuilderConfiguration AllowedCharacters { get; set; } = new AllowedCharactersBuilderConfiguration
        {
            AllowSpecialChars = false
        };
    }
}