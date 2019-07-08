using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class DataSourceMapping : ClassMapping<DataSourceEntity>, IMapping
    {
        public DataSourceMapping()
        {
            Table("`DataSource`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.DataSource, map =>
            {
                map.NotNullable(true);
                map.Type<EnumStringType<DataSourceEnum>>();
            });

            ManyToOne(x => x.ExternalLoginProvider, map =>
            {
                map.NotNullable(false);
                map.Column("`ExternalLoginProviderId`");
            });
        }
    }
}
