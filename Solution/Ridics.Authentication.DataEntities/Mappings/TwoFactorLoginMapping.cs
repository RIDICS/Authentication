using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class TwoFactorLoginMapping : ClassMapping<TwoFactorLoginEntity>, IMapping
    {
        public TwoFactorLoginMapping()
        {
            Table("`TwoFactorLogin`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            ManyToOne(x => x.User, map =>
            {
                map.NotNullable(true);
                map.Column("`UserId`");
            });

            Property(x => x.TokenProvider, map => { map.NotNullable(true); });

            Property(x => x.Token, map => { map.NotNullable(true); });

            Property(x => x.CreateTime, map => { map.NotNullable(true); });
        }
    }
}