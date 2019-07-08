using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class UriMapping : ClassMapping<UriEntity>, IMapping
    {
        public UriMapping()
        {
            Table("`Uri`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Uri, map => { map.NotNullable(true); });

            Set(x => x.UriTypes, map =>
            {
                map.Table("`Uri_UriType`");
                map.Key(keyMapper => keyMapper.Column("`UriId`"));
            }, rel => rel.ManyToMany(m => m.Column("`UriTypeId`")));

            ManyToOne(x => x.Client, map =>
            {
                map.Column("`ClientId`");
                map.NotNullable(true);
            });
        }
    }
}