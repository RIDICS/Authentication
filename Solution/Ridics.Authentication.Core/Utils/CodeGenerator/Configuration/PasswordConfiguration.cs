namespace Ridics.Authentication.Core.Utils.CodeGenerator.Configuration
{
    public class PasswordConfiguration : CodeGeneratorConfigurationBase
    {
        public override int CodeLength { get; set; } = 16;

        public override AllowedCharactersBuilderConfiguration AllowedCharacters { get; set; } = new AllowedCharactersBuilderConfiguration
        {
            AllowNumeric = true,
            AllowLowerAlphabet = true,
            AllowUpperAlphabet = true,
            AllowSpecialChars = true,
        }; // all set true independently on base configuration for security reason
    }
}