using System;

namespace Ridics.Authentication.DataEntities.Entities
{
    public abstract class SecretEntity : IEquatable<SecretEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Description { get; set; }

        public virtual string Value { get; set; }

        public virtual DateTime? Expiration { get; set; }

        public virtual bool Equals(SecretEntity other)
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
            return Equals((SecretEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}