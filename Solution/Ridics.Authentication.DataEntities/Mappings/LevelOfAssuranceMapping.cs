using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class LevelOfAssuranceMapping : ClassMapping<LevelOfAssuranceEntity>, IMapping
    {
        public LevelOfAssuranceMapping()
        {
            Table("LevelOfAssurance");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Type<EnumStringType<LevelOfAssuranceEnum>>();
            });

            Property(x => x.Level, map =>
            {
                map.NotNullable(true);
            });
        }
    }
}
