using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.Utils.CodeGenerator.Configuration;
using Ridics.Authentication.Core.Utils.CodeGenerator.Generic;
using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Ridics.Authentication.Core.Utils.CodeGenerator.ContactConfirm
{
    public class SmsConfirmCodeGenerator : GeneratorBase, IContactConfirmCodeGenerator
    {
        public SmsConfirmCodeGenerator(
            IGenericCodeGenerator genericCodeGenerator,
            SmsConfirmConfiguration configuration
        ) : base(genericCodeGenerator, configuration)
        {
        }

        public ContactTypeEnumModel SupportedContactType => ContactTypeEnumModel.Phone;
    }
}