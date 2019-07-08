using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class IdentityResourceMapping : SubclassMapping<IdentityResourceEntity>, IMapping
    {
        public IdentityResourceMapping()
        {
            DiscriminatorValue("IdentityResource");

            Set(x => x.Clients, map =>
            {
                map.Table("`Client_IdentityResource`");
                map.Key(keyMapper => keyMapper.Column("`IdentityResourceId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ClientId`")));
        }
    }
}