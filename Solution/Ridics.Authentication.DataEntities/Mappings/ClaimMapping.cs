using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ClaimMapping : ClassMapping<ClaimEntity>, IMapping
    {
        public ClaimMapping()
        {
            Table("`Claim`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            ManyToOne(x => x.User, map =>
            {
                map.NotNullable(true);
                map.Column("`UserId`");
            });

            Property(x => x.Value, map => { map.NotNullable(true); });

            ManyToOne(x => x.ClaimType, map =>
            {
                map.NotNullable(true);
                map.Column("`ClaimTypeId`");
            });
        }
    }
}