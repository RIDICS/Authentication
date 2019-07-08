using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ApiResourceMapping : SubclassMapping<ApiResourceEntity>, IMapping
    {
        public ApiResourceMapping()
        {
            DiscriminatorValue("ApiResource");

            Set(x => x.ApiSecrets, map =>
            {
                map.Table("`Secret`");
                map.Key(keyMapper => keyMapper.Column("`ResourceId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());

            Set(x => x.Scopes, map =>
            {
                map.Table("`Scope`");
                map.Key(keyMapper => keyMapper.Column("`ResourceId`"));
                map.Inverse(true);
                map.Cascade(Cascade.DeleteOrphans);
            }, rel => rel.OneToMany());
        }
    }
}