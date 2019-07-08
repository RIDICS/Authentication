using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ClaimTypeMapping : ClassMapping<ClaimTypeEntity>, IMapping
    {
        public ClaimTypeMapping()
        {
            Table("`ClaimType`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Description);

            ManyToOne(x => x.Type, map => { map.Column("`ClaimTypeEnumId`"); });

            Set(x => x.Claims, map =>
            {
                map.Table("`Claim`");
                map.Key(keyMapper => keyMapper.Column("`ClaimTypeId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());

            Set(x => x.Scopes, map =>
            {
                map.Table("`Scope_ClaimType`");
                map.Key(keyMapper => keyMapper.Column("`ClaimTypeId`"));
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.ManyToMany(m => m.Column("`ScopeId`")));

            Set(x => x.Resources, map =>
            {
                map.Table("`Resource_ClaimType`");
                map.Key(keyMapper => keyMapper.Column("`ClaimTypeId`"));
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.ManyToMany(m => m.Column("`ResourceId`")));
        }
    }
}