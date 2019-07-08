using Ridics.Authentication.Core.Utils.CodeGenerator.Configuration;
using Ridics.Authentication.Core.Utils.CodeGenerator.Generic;
using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Ridics.Authentication.Core.Utils.CodeGenerator.User
{
    public class VerificationCodeGenerator : GeneratorBase
    {
        public VerificationCodeGenerator(
            IGenericCodeGenerator genericCodeGenerator,
            VerificationCodeConfiguration configuration
        ) : base(genericCodeGenerator, configuration)
        {
        }
    }
}