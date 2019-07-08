using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ApiResourceEntity : ResourceEntity
    {
        public virtual ISet<SecretEntity> ApiSecrets { get; set; }

        public virtual ISet<ScopeEntity> Scopes { get; set; }
    }
}