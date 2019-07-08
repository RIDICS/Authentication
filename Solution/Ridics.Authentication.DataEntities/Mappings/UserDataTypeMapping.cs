using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class UserDataTypeMapping : ClassMapping<UserDataTypeEntity>, IMapping
    {
        public UserDataTypeMapping()
        {
            Table("`UserDataType`");

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.DataTypeValue, map => map.NotNullable(true));

            Set(x => x.UserData, map =>
            {
                map.Table("`UserData`");
                map.Key(keyMapper => keyMapper.Column("`TypeId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, map => { map.OneToMany(); });
        }
    }
}