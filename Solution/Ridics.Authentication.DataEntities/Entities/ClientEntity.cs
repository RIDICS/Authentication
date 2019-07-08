using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ClientEntity : IEquatable<ClientEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ISet<SecretEntity> Secrets { get; set; }

        public virtual ISet<GrantTypeEntity> AllowedGrantTypes { get; set; }

        public virtual ISet<UriEntity> UriList { get; set; }

        public virtual ISet<IdentityResourceEntity> AllowedIdentityResources { get; set; }

        public virtual ISet<ScopeEntity> AllowedScopes { get; set; }

        public virtual string DisplayUrl { get; set; }

        public virtual string LogoUrl { get; set; }

        public virtual bool RequireConsent { get; set; }

        public virtual ISet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual bool AllowAccessTokensViaBrowser { get; set; }

        public virtual bool Equals(ClientEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ClientEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}