using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ApiSecretMapping : SubclassMapping<ApiSecretEntity>, IMapping
    {
        public ApiSecretMapping()
        {
            DiscriminatorValue("ApiSecret");

            ManyToOne(x => x.ApiResource, map => { map.Column("`ResourceId`"); });
        }
    }
}