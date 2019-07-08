using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ScopeMapping : ClassMapping<ScopeEntity>, IMapping
    {
        public ScopeMapping()
        {
            Table("`Scope`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Description);

            Set(x => x.ClaimTypes, map =>
            {
                map.Table("`Scope_ClaimType`");
                map.Key(keyMapper => keyMapper.Column("`ScopeId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ClaimTypeId`")));

            ManyToOne(x => x.ApiResource, map => { map.Column("`ResourceId`"); });

            Property(x => x.Required);

            Property(x => x.ShowInDiscoveryDocument);
        }
    }
}