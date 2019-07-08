using System;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ExternalLoginEntity : IEquatable<ExternalLoginEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual ExternalLoginProviderEntity Provider { get; set; }

        public virtual UserEntity User { get; set; }

        public virtual string ProviderKey { get; set; }

        public virtual bool Equals(ExternalLoginEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExternalLoginProviderEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}