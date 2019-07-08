namespace Ridics.Authentication.Core.Utils.CodeGenerator.Configuration
{
    public class AllowedCharactersBuilderConfiguration
    {
        public bool AllowNumeric { get; set; } = true;

        public bool AllowUpperAlphabet { get; set; } = true;

        public bool AllowLowerAlphabet { get; set; } = true;

        public bool AllowSpecialChars { get; set; } = true;
    }
}