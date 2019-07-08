using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class GrantTypeEntity : IEquatable<GrantTypeEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string DisplayName { get; set; }

        public virtual string Value { get; set; }

        public virtual ISet<ClientEntity> Clients { get; set; }

        public virtual bool Equals(GrantTypeEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GrantTypeEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}