using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.Utils.CodeGenerator.Generic;

namespace Ridics.Authentication.Core.Utils.CodeGenerator.ContactConfirm
{
    public interface IContactConfirmCodeGenerator : IGenerator
    {
        ContactTypeEnumModel SupportedContactType { get; }
    }
}
