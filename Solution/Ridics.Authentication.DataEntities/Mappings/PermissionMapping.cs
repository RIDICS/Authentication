using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class PermissionMapping : ClassMapping<PermissionEntity>, IMapping
    {
        public PermissionMapping()
        {
            Table("`Permission`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Description);

            Set(x => x.Roles, map =>
            {
                map.Table("`Role_Permission`");
                map.Key(keyMapper => keyMapper.Column("`PermissionId`"));
            }, rel => rel.ManyToMany(m => m.Column("`RoleId`")));
        }
    }
}
