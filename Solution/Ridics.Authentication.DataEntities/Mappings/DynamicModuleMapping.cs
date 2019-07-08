using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.DataTypes;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class DynamicModuleMapping : ClassMapping<DynamicModuleEntity>, IMapping
    {
        public DynamicModuleMapping()
        {
            Table("`DynamicModule`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name);

            Property(x => x.ModuleGuid, map => map.NotNullable(true));

            Property(x => x.ConfigurationVersion, map =>
            {
                map.NotNullable(true);
                map.Type<VersionType>();
            });

            Property(x => x.Configuration);

            Property(x => x.ConfigurationChecksum, map => map.NotNullable(true));

            Set(x => x.DynamicModuleBlobs, map =>
            {
                map.Table("`DynamicModuleBlob`");
                map.Key(keyMapper => keyMapper.Column("`DynamicModuleId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, map => { map.OneToMany(); });
        }
    }
}
