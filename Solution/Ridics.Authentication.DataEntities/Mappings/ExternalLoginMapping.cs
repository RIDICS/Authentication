using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ExternalLoginMapping : ClassMapping<ExternalLoginEntity>, IMapping
    {
        public ExternalLoginMapping()
        {
            Table("`ExternalLogin`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            ManyToOne(x => x.Provider, map =>
            {
                map.NotNullable(true);
                map.Column("`ProviderId`");
            });

            ManyToOne(x => x.User, map =>
            {
                map.NotNullable(true);
                map.Column("`UserId`");
            });

            Property(x => x.ProviderKey, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });
        }
    }
}
