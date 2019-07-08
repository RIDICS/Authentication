using Ridics.Authentication.Core.Utils.CodeGenerator.Configuration;
using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Ridics.Authentication.Core.Utils.CodeGenerator
{
    public static class AllowedCharactersBuilderExtensions
    {
        public static char[] BuildFromConfiguration(this AllowedCharactersBuilder builder, AllowedCharactersBuilderConfiguration configuration)
        {
            if (configuration.AllowNumeric)
            {
                builder.AddNumeric();
            }

            if (configuration.AllowUpperAlphabet)
            {
                builder.AddUpperAlphabet();
            }

            if (configuration.AllowLowerAlphabet)
            {
                builder.AddLowerAlphabet();
            }

            if (configuration.AllowSpecialChars)
            {
                builder.AddSpecialChars();
            }

            return builder.Build();
        }
    }
}
