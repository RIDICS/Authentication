namespace Ridics.Authentication.Core.Utils.CodeGenerator.Configuration
{
    public class EmailConfirmConfiguration : CodeGeneratorConfigurationBase
    {
        public override int CodeLength { get; set; } = 10;

        public override AllowedCharactersBuilderConfiguration AllowedCharacters { get; set; } = new AllowedCharactersBuilderConfiguration
        {
            AllowSpecialChars = false
        };
    }
}