using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ResourcePermissionMapping : ClassMapping<ResourcePermissionEntity>, IMapping
    {
        public ResourcePermissionMapping()
        {
            Table("`ResourcePermission`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            ManyToOne(x => x.ResourceTypeAction, map =>
            {
                map.NotNullable(true);
                map.Column("`ResourcePermissionTypeActionId`");
            });

            Property(x => x.ResourceId);

            Set(x => x.Users, map =>
            {
                map.Table("`User_ResourcePermission`");
                map.Key(keyMapper => keyMapper.Column("`ResourcePermissionId`"));
            }, rel => rel.ManyToMany(m => m.Column("`UserId`")));

            Set(x => x.Roles, map =>
            {
                map.Table("`Role_ResourcePermission`");
                map.Key(keyMapper => keyMapper.Column("`ResourcePermissionId`"));
            }, rel => rel.ManyToMany(m => m.Column("`RoleId`")));

        }
    }
}