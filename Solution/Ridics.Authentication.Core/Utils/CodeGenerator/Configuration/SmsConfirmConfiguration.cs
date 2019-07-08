namespace Ridics.Authentication.Core.Utils.CodeGenerator.Configuration
{
    public class SmsConfirmConfiguration : CodeGeneratorConfigurationBase
    {
        public override int CodeLength { get; set; } = 5;

        public override AllowedCharactersBuilderConfiguration AllowedCharacters { get; set; } = new AllowedCharactersBuilderConfiguration
        {
            AllowSpecialChars = false
        };
    }
}