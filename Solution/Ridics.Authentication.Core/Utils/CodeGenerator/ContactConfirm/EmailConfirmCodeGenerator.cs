using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.Utils.CodeGenerator.Configuration;
using Ridics.Authentication.Core.Utils.CodeGenerator.Generic;
using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Ridics.Authentication.Core.Utils.CodeGenerator.ContactConfirm
{
    public class EmailConfirmCodeGenerator : GeneratorBase, IContactConfirmCodeGenerator
    {
        public EmailConfirmCodeGenerator(
            IGenericCodeGenerator genericCodeGenerator,
            EmailConfirmConfiguration configuration
        ) : base(genericCodeGenerator, configuration)
        {
        }

        public ContactTypeEnumModel SupportedContactType => ContactTypeEnumModel.Email;
    }
}