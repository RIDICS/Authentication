using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class DynamicModuleBlobMapping : ClassMapping<DynamicModuleBlobEntity>, IMapping
    {
        public DynamicModuleBlobMapping()
        {
            Table("`DynamicModuleBlob`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            ManyToOne(x => x.DynamicModule, map =>
            {
                map.NotNullable(true);
                map.Column("`DynamicModuleId`");
            });

            Property(x => x.Type, map =>
            {
                map.NotNullable(true);
                map.Type<EnumStringType<DynamicModuleBlobEnum>>();
            });

            ManyToOne(x => x.File, map => { map.Column("`FileId`"); });

            Property(x => x.LastChange, map =>
            {
                map.NotNullable(true);
            });
        }
    }
}
