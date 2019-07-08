using Ridics.Authentication.Core.Utils.CodeGenerator.Configuration;
using Ridics.Authentication.Core.Utils.CodeGenerator.Generic;
using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Ridics.Authentication.Core.Utils.CodeGenerator.User
{
    public class PasswordGenerator : GeneratorBase
    {
        public PasswordGenerator(
            IGenericCodeGenerator genericCodeGenerator,
            PasswordConfiguration configuration
        ) : base(genericCodeGenerator, configuration)
        {
        }

        protected override char[] GetAllowedChars(CodeGeneratorConfigurationBase configuration)
        {
            return new AllowedCharactersBuilder()
                .BuildFromConfiguration(configuration.AllowedCharacters); //HACK load requirements from Identity and check it
        }
    }
}