using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ResourcePermissionTypeMapping : ClassMapping<ResourcePermissionTypeEntity>, IMapping
    {
        public ResourcePermissionTypeMapping()
        {
            Table("`ResourcePermissionType`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Description);

            Set(x => x.ResourcePermissionTypeActions, map =>
            {
                map.Table("`ResourcePermissionTypeAction`");
                map.Key(keyMapper => keyMapper.Column("`ResourcePermissionTypeId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, map => { map.OneToMany(); });
        }
    }
}