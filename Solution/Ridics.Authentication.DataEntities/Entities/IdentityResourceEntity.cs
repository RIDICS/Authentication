using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class IdentityResourceEntity : ResourceEntity
    {
        public virtual ISet<ClientEntity> Clients { get; set; }
    }
}