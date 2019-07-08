using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class UserMapping : ClassMapping<UserEntity>, IMapping
    {
        public UserMapping()
        {
            Table("`User`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));
            
            Property(x => x.Username, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.PasswordHash, map => { map.NotNullable(true); });

            Property(x => x.SecurityStamp);

            Property(x => x.TwoFactorEnabled, map => { map.NotNullable(true); });

            Property(x => x.LockoutEndDateUtc);

            Property(x => x.LockoutEnabled, map => { map.NotNullable(true); });

            Property(x => x.AccessFailedCount, map => { map.NotNullable(true); });

            Property(x => x.LastChange, map => { map.NotNullable(true); });

            Property(x => x.TwoFactorProvider);

            Property(x => x.VerificationCode);

            Property(x => x.VerificationCodeCreateTime);

            Property(x => x.PasswordResetToken);

            Property(x => x.PasswordResetTokenCreateTime);
            
            Set(x => x.ExternalLogins, map =>
            {
                map.Table("`ExternalLogin`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());

            Set(x => x.ExternalIdentities, map =>
            {
                map.Table("`UserExternalIdentity`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, map => { map.OneToMany(); });

            Set(x => x.Roles, map =>
            {
                map.Table("`User_Role`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
            }, rel => rel.ManyToMany(m => m.Column("`RoleId`")));

            Set(x => x.ResourcePermissions, map =>
            {
                map.Table("`User_ResourcePermission`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ResourcePermissionId`")));

            Set(x => x.ResourcePermissionTypeActions, map =>
            {
                map.Table("`User_ResourcePermissionTypeAction`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ResourcePermissionTypeActionId`")));

            Set(x => x.UserData, map =>
            {
                map.Table("`UserData`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
                map.Inverse(true);
                map.Cascade(Cascade.Remove);
            }, map => { map.OneToMany(); });

            Set(x => x.UserContacts, map =>
            {
                map.Table("`UserContact`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
                map.Inverse(true);
                map.Cascade(Cascade.Remove);
            }, map => { map.OneToMany(); });

            Set(x => x.UserDataVerifiedByThisUser, map =>
            {
                map.Table("`UserData`");
                map.Key(keyMapper => keyMapper.Column("`VerifiedBy`"));
                map.Inverse(true);
            }, map => { map.OneToMany(); });

            Set(x => x.UserClaims, map =>
            {
                map.Table("`Claim`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());

            Set(x => x.PersistedGrants, map =>
            {
                map.Table("`PersistedGrant`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());


            Set(x => x.TwoFactorLogins, map =>
            {
                map.Table("`TwoFactorLogins`");
                map.Key(keyMapper => keyMapper.Column("`UserId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());
        }
    }
}