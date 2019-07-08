using NHibernate.Mapping.ByCode.Conformist;
using Ridics.Authentication.DataEntities.Entities;

namespace Ridics.Authentication.DataEntities.Mappings
{
    public class ClientSecretMapping : SubclassMapping<ClientSecretEntity>, IMapping
    {
        public ClientSecretMapping()
        {
            DiscriminatorValue("ClientSecret");

            ManyToOne(x => x.Client, map => { map.Column("`ClientId`"); });
        }
    }
}