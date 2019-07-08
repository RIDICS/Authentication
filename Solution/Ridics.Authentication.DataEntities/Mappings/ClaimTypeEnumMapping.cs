using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ClaimTypeEnumMapping : ClassMapping<ClaimTypeEnumEntity>, IMapping
    {
        public ClaimTypeEnumMapping()
        {
            Table("`ClaimTypeEnum`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Set(x => x.ClaimTypes, map =>
            {
                map.Table("`ClaimTypeEnum`");
                map.Key(keyMapper => keyMapper.Column("`ClaimTypeEnumId`"));
            }, rel => rel.OneToMany());
        }
    }
}