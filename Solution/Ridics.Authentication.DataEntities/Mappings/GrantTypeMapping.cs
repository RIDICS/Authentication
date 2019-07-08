using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class GrantTypeMapping : ClassMapping<GrantTypeEntity>, IMapping
    {
        public GrantTypeMapping()
        {
            Table("`GrantType`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Value, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.DisplayName);

            Set(x => x.Clients, map =>
            {
                map.Table("`Client_GrantType`");
                map.Key(keyMapper => keyMapper.Column("`GrantTypeId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ClientId`")));
        }
    }
}