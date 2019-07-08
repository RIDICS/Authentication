using Ridics.Authentication.Core.Utils.CodeGenerator.Configuration;
using Ridics.Authentication.Core.Utils.CodeGenerator.Generic;
using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Ridics.Authentication.Core.Utils.CodeGenerator.User
{
    public class UsernameGenerator : GeneratorBase
    {
        public UsernameGenerator(
            IGenericCodeGenerator generator,
            UsernameConfiguration configuration
        ) : base(generator, configuration)
        {
        }
    }
}