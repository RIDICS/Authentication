using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Utils.Formatter
{
    public interface IContactFormatter
    {
        ContactTypeEnumModel Type { get; }

        string FormatContact(string contact);
    }
}