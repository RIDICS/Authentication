using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class UserDataMapping : ClassMapping<UserDataEntity>, IMapping
    {
        public UserDataMapping()
        {
            Table("`UserData`");

            Lazy(true);

            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Value);

            Property(x => x.ActiveFrom, map => map.NotNullable(true));

            Property(x => x.ActiveTo, map => map.NotNullable(false));

            ManyToOne(x => x.User, map =>
            {
                map.Column("`UserId`");
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.UserDataType, map =>
            {
                map.Column("`TypeId`");
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.VerifiedBy, map =>
            {
                map.Column("`VerifiedBy`");
                map.Cascade(Cascade.None);
            });

            ManyToOne(x => x.ParentUserData, map =>
            {
                map.Column("`DataParentId`");
                map.Cascade(Cascade.None);
            });

            Bag(x => x.ChildrenUserData, map =>
            {
                map.Table("`UserDataType`");
                map.Key(x => x.Column("`DataParentId`"));
                map.Inverse(true);
            }, map => { map.OneToMany(); });

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
