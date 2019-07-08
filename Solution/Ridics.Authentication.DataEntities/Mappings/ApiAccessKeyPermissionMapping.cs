using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ApiAccessKeyPermissionMapping : ClassMapping<ApiAccessKeyPermissionEntity>, IMapping
    {
        public ApiAccessKeyPermissionMapping()
        {
            Table("`ApiAccessKeyPermission`");
            Lazy(true);

            Id(x => x.Id, map => map.Generator(Generators.Identity));

            Property(x => x.Permission, map => map.NotNullable(true));

            ManyToOne(x => x.ApiAccessKey, map =>
            {
                map.Column("`ApiAccessKeyId`");
                map.Cascade(Cascade.None);
            });
        }
    }
}