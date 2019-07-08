namespace Ridics.Authentication.Core.Utils.CodeGenerator.Configuration
{
    public abstract class CodeGeneratorConfigurationBase
    {
        public virtual int CodeLength { get; set; }

        public virtual AllowedCharactersBuilderConfiguration AllowedCharacters { get; set; }
    }
}