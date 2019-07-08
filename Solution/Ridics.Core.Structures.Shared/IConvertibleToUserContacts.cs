namespace Ridics.Core.Structures.Shared
{
    public interface IConvertibleToUserContacts
    {
        string Email { get; set; }

        ContactLevelOfAssuranceEnum EmailLevelOfAssurance { get; set; }

        string EmailConfirmCode { get; set; }

        string PhoneNumber { get; set; }

        ContactLevelOfAssuranceEnum PhoneLevelOfAssurance { get; set; }

        string PhoneNumberConfirmCode { get; set; }
    }
}