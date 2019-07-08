using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Ridics.Authentication.Core.Models;
using Ridics.Core.Structures;
using Ridics.Core.Structures.Shared;

namespace Ridics.Authentication.Service.Authentication.Identity.Models
{
    public class ApplicationUser : IdentityUser<int>, IConvertibleToUserContacts
    {
        public Guid MasterUserId { get; set; }

        //public string FirstName { get; set; }

        //public string LastName { get; set; }

        public string TwoFactorProvider { get; set; }

        public DateTime LastChange { get; set; }

        public string VerificationCode { get; set; }

        public DateTime? VerificationCodeCreateTime { get; set; }

        public string AuthenticatorKey { get; set; }

        public ContactLevelOfAssuranceEnum EmailLevelOfAssurance { get; set; }

        public string EmailConfirmCode { get; set; }

        public ContactLevelOfAssuranceEnum PhoneLevelOfAssurance { get; set; }

        public string PhoneNumberConfirmCode { get; set; }

        public IList<TwoFactorLoginModel> TwoFactorLogins { get; set; }

        public List<Claim> UserClaims { get; set; }

        public List<ApplicationRole> Roles { get; set; }

        public List<ResourcePermissionModel> ResourcePermissions { get; set; }

        public List<ResourcePermissionTypeActionModel> ResourcePermissionTypeActions { get; set; }

        public List<string> Permissions { get; set; }

        public List<ExternalUserLoginModel> ExternalUserLogins { get; set; }

        public IList<UserDataModel> UserData { get; set; }
    }
}