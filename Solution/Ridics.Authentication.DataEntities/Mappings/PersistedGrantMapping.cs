using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class PersistedGrantMapping : ClassMapping<PersistedGrantEntity>, IMapping
    {
        public PersistedGrantMapping()
        {
            Table("`PersistedGrant`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Key, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
                map.Column("`Key`");
            });

            Property(x => x.Type);

            ManyToOne(x => x.User, map => { map.Column("`UserId`"); });

            ManyToOne(x => x.Client, map => { map.Column("`ClientId`"); });

            Property(x => x.CreationTime);

            Property(x => x.ExpirationTime);

            Property(x => x.Data, map => { map.Type(NHibernateUtil.StringClob); });
        }
    }
}