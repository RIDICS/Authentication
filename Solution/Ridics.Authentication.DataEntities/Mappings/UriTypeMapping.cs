using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class UriTypeMapping : ClassMapping<UriTypeEntity>, IMapping
    {
        public UriTypeMapping()
        {
            Table("`UriType`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Value, map => map.NotNullable(true));

            Set(x => x.UriEntities, map =>
            {
                map.Table("`Uri_UriType`");
                map.Key(keyMapper => keyMapper.Column("`UriTypeId`"));
                map.Inverse(true);
            }, map => { map.ManyToMany(m => m.Column("`UriId`")); });
        }
    }
}