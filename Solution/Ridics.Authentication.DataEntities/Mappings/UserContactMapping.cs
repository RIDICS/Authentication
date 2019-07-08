using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NHibernate.Type;
using Ridics.Authentication.DataEntities.Entities;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class UserContactMapping : ClassMapping<UserContactEntity>, IMapping
    {
        public UserContactMapping()
        {
            Table("`UserContact`");

            Lazy(true);

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Value);

            Property(x => x.Type, map => map.Type<EnumStringType<ContactTypeEnum>>());
            
            Property(x => x.ConfirmCode);

            Property(x => x.ActiveFrom, map => map.NotNullable(true));

            Property(x => x.ActiveTo, map => map.NotNullable(false));

            Property(x => x.ConfirmCodeChangeTime);

            ManyToOne(x => x.User, map =>
            {
                map.Column("`UserId`");
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.LevelOfAssurance, map =>
            {
                map.Column("`LevelOfAssuranceId`");
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.DataSource, map =>
            {
                map.Column("`DataSourceId`");
                map.Cascade(Cascade.None);
            });
        }
    }
}
