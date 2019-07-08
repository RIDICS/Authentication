using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class SecretMapping : ClassMapping<SecretEntity>, IMapping
    {
        public SecretMapping()
        {
            Table("`Secret`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Discriminator(x =>
            {
                x.Force(true);
                x.NotNullable(true);
                x.Column("`Discriminator`");
            });

            Property(x => x.Value, map => { map.NotNullable(true); });

            Property(x => x.Description);

            Property(x => x.Expiration);
        }
    }
}