using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class UserExternalIdentityMapping : ClassMapping<UserExternalIdentityEntity>, IMapping
    {
        public UserExternalIdentityMapping()
        {
            Table("`UserExternalIdentity`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            ManyToOne(x => x.ExternalIdentityType, map =>
            {
                map.NotNullable(true);
                map.Column("`ExternalIdentityId`");
            });

            ManyToOne(x => x.User, map =>
            {
                map.NotNullable(true);
                map.Column("`UserId`");
            });

            Property(x => x.ExternalIdentity, map =>
            {
                map.NotNullable(true);
                map.Unique(true);
            });
        }
    }
}
