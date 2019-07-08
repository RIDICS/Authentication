using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ResourceMapping : ClassMapping<ResourceEntity>, IMapping
    {
        public ResourceMapping()
        {
            Table("`Resource`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Discriminator(x =>
            {
                x.Force(true);
                x.NotNullable(true);
                x.Column("`Discriminator`");
            });

            Property(x => x.Name, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });

            Property(x => x.Description);

            Set(x => x.ClaimTypes, map =>
            {
                map.Table("`Resource_ClaimType`");
                map.Key(keyMapper => keyMapper.Column("`ResourceId`"));
            }, rel => rel.ManyToMany(m => m.Column("`ClaimTypeId`")));

            Property(x => x.Required);

            Property(x => x.ShowInDiscoveryDocument);
        }
    }
}