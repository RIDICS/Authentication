using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Utils.Validator
{
    public interface IContactValidator
    {
        ContactTypeEnumModel Type { get; }

        bool Validate(string contact);
    }
}
