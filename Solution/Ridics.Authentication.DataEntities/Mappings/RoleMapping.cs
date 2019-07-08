using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class RoleMapping : ClassMapping<RoleEntity>, IMapping
    {
        public RoleMapping()
        {
            Table("`Role`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Description);

            Property(x => x.AuthenticationServiceOnly);

            Set(x => x.User, map =>
            {
                map.Table("`User_Role`");
                map.Key(keyMapper => keyMapper.Column("`RoleId`"));
            }, rel => rel.ManyToMany(m => m.Column("`UserId`")));

            Set(x => x.Permissions, map =>
            {
                map.Table("`Role_Permission`");
                map.Key(keyMapper => keyMapper.Column("`RoleId`"));
            }, rel => rel.ManyToMany(m => m.Column("`PermissionId`")));

            Set(x => x.ResourcePermissions, map =>
            {
                map.Table("`Role_ResourcePermission`");
                map.Key(keyMapper => keyMapper.Column("`RoleId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ResourcePermissionId`")));

            Set(x => x.ResourcePermissionTypeActions, map =>
            {
                map.Table("`Role_ResourcePermissionTypeAction`");
                map.Key(keyMapper => keyMapper.Column("`RoleId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ResourcePermissionTypeActionId`")));
        }
    }
}