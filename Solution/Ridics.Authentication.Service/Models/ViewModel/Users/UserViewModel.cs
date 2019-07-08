using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Ridics.Authentication.Service.Models.ViewModel.Account;
using Ridics.Authentication.Service.Models.ViewModel.Permission;
using Ridics.Authentication.Service.Models.ViewModel.Roles;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Models.ViewModel.Users
{
    public class UserViewModel : IConvertibleToUserData, IConvertibleToUserContacts
    {
        public int Id { get; set; }

        [Display(Name = "username")]
        [Required(ErrorMessage = "username-required")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        public Guid? MasterUserId { get; set; }

        [Display(Name = "first-name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Display(Name = "last-name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Display(Name = "email")]
        [Required(ErrorMessage = "email-required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "wrong-email")]
        [EmailAddress(ErrorMessage = "wrong-email")]
        public string Email { get; set; }

        [Display(Name = "assigned-roles")]
        public List<RoleViewModel> Roles { get; set; }

        [Display(Name = "resource-permissions")]
        public List<ResourcePermissionViewModel> ResourcePermissions { get; set; }

        [Display(Name = "resource-permission-types")]
        public List<ResourcePermissionTypeActionViewModel> ResourcePermissionTypeActions { get; set; }

        [Display(Name = "last-change")]
        public DateTime LastChange { get; set; }

        [Display(Name = "claims")]
        public List<Claim> UserClaims { get; set; }

        [Display(Name = "email-loa")]
        public ContactLevelOfAssuranceEnum EmailLevelOfAssurance { get; set; }

        public string SecurityStamp { get; set; }

        [Display(Name = "phone-number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "phone-loa")]
        public ContactLevelOfAssuranceEnum PhoneLevelOfAssurance { get; set; }

        [Display(Name = "enabled-twofactor")]
        public bool TwoFactorEnabled { get; set; }

        //TODO Add localized name to other displayed properties

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public IList<TwoFactorLoginViewModel> TwoFactorLogins { get; set; }

        [Display(Name = "twofactor-provider")]
        public string TwoFactorProvider { get; set; }

        [Display(Name = "verification-code")]
        public string VerificationCode { get; set; }

        [Display(Name = "verification-code-created")]
        public DateTime? VerificationCodeCreateTime { get; set; }

        [Display(Name = "twofactor-providers")]
        public IList<string> TwoFactorProviders { get; set; }

        [Display(Name = "external-logins")]
        public IList<ExternalLoginViewModel> ExternalUserLogins { get; set; }


        public string GetFullName() // TODO consider removing in favour of user-entered FullName property
        {
            return $"{FirstName} {LastName}";
        }

        //TODO remove hub related properties
        [DataType(DataType.Text)]
        public UserAddressingWays Title { get; set; }

        [DataType(DataType.Text)]
        public string Prefix { get; set; }

        [DataType(DataType.Text)]
        public string SecondName { get; set; }

        [DataType(DataType.Text)]
        public string FullName { get; set; }

        [DataType(DataType.Text)]
        public string Suffix { get; set; }

        [Display(Name = "email-confirm-code")]
        public string EmailConfirmCode { get; set; }

        [Display(Name = "phone-confirm-code")]
        public string PhoneNumberConfirmCode { get; set; }
    }
}
