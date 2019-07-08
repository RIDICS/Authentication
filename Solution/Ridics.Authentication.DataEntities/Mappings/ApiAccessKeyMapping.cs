using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ApiAccessKeyMapping : ClassMapping<ApiAccessKeyEntity>, IMapping
    {
        public ApiAccessKeyMapping()
        {
            Table("`ApiAccessKey`");
            Lazy(true);
            Id(x => x.Id, map => map.Generator(Generators.Identity));
            Property(x => x.Name, map => map.NotNullable(true));
            Property(x => x.ApiKeyHash, map => map.NotNullable(true));
            Property(x => x.HashAlgorithm, map => map.NotNullable(true));
            Bag(x => x.Permissions, colmap =>
            {
                colmap.Table("`ApiAccessKeyPermission`");
                colmap.Key(x => x.Column("`ApiAccessKeyId`"));
                colmap.Inverse(true);
                colmap.Cascade(Cascade.All | Cascade.DeleteOrphans);
            }, map => { map.OneToMany(); });
        }
    }
}