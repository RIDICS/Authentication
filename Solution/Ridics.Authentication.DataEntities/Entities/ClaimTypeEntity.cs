using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ClaimTypeEntity : IEquatable<ClaimTypeEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual ClaimTypeEnumEntity Type { get; set; }

        public virtual string Description { get; set; }

        public virtual ISet<ClaimEntity> Claims { get; set; }

        public virtual ISet<ScopeEntity> Scopes { get; set; }

        public virtual ISet<ResourceEntity> Resources { get; set; }

        public virtual bool Equals(ClaimTypeEntity other)
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
            return Equals((ClaimTypeEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}