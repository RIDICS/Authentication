using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ResourcePermissionTypeActionMapping : ClassMapping<ResourcePermissionTypeActionEntity>, IMapping
    {
        public ResourcePermissionTypeActionMapping()
        {
            Table("`ResourcePermissionTypeAction`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Description);

            ManyToOne(x => x.ResourcePermissionType, map =>
            {
                map.NotNullable(true);
                map.Column("`ResourcePermissionTypeId`");
            });

            Set(x => x.ResourcePermissions, map =>
            {
                map.Table("`ResourcePermission`");
                map.Key(keyMapper => keyMapper.Column("`ResourcePermissionTypeActionId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, map => { map.OneToMany(); });

            Set(x => x.Users, map =>
            {
                map.Table("`User_ResourcePermissionTypeAction`");
                map.Key(keyMapper => keyMapper.Column("`ResourcePermissionTypeActionId`"));
            }, rel => rel.ManyToMany(m => m.Column("`UserId`")));

            Set(x => x.Roles, map =>
            {
                map.Table("`Role_ResourcePermissionTypeAction`");
                map.Key(keyMapper => keyMapper.Column("`ResourcePermissionTypeActionId`"));
            }, rel => rel.ManyToMany(m => m.Column("`RoleId`")));
        }
    }
}