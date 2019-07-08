using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class UserEntity : IEquatable<UserEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual bool TwoFactorEnabled { get; set; }

        public virtual DateTime? LockoutEndDateUtc { get; set; }

        public virtual bool LockoutEnabled { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual string Username { get; set; }

        public virtual string PasswordHash { get; set; }

        public virtual string SecurityStamp { get; set; }

        public virtual DateTime LastChange { get; set; }

        public virtual string TwoFactorProvider { get; set; }

        public virtual string VerificationCode { get; set; }

        public virtual DateTime? VerificationCodeCreateTime { get; set; }

        public virtual string PasswordResetToken { get; set; }

        public virtual DateTime? PasswordResetTokenCreateTime { get; set; }

        public virtual ISet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual ISet<TwoFactorLoginEntity> TwoFactorLogins { get; set; }

        public virtual ISet<RoleEntity> Roles { get; set; }

        public virtual ISet<ResourcePermissionEntity> ResourcePermissions { get; set; }

        public virtual ISet<ResourcePermissionTypeActionEntity> ResourcePermissionTypeActions { get; set; }

        public virtual ISet<ClaimEntity> UserClaims { get; set; }

        public virtual ISet<ExternalLoginEntity> ExternalLogins { get; set; }

        public virtual ISet<UserContactEntity> UserContacts { get; set; }

        public virtual ISet<UserDataEntity> UserData { get; set; }

        public virtual ISet<UserDataEntity> UserDataVerifiedByThisUser { get; set; }

        public virtual ISet<UserExternalIdentityEntity> ExternalIdentities { get; set; }


        public virtual bool Equals(UserEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((UserEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}