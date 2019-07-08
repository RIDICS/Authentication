using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class UserModel : UserInfoModel
    {
        public IList<RoleModel> Roles { get; set; }

        public IList<ResourcePermissionInfoModel> ResourcePermissions { get; set; }

        public IList<ResourcePermissionTypeActionInfoModel> ResourcePermissionTypeActions { get; set; }

        public IList<ClaimModel> UserClaims { get; set; }

        public IList<TwoFactorLoginModel> TwoFactorLogins { get; set; }

        public IList<ExternalUserLoginModel> ExternalUserLogins { get; set; }

        public IList<UserContactModel> UserContacts { get; set; }

        public IList<UserDataModel> UserData { get; set; }

        public IList<UserExternalIdentityModel> ExternalIdentities { get; set; }
    }
}