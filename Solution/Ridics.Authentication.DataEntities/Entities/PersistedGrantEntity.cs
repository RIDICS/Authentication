using System;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class PersistedGrantEntity : IEquatable<PersistedGrantEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Key { get; set; }

        public virtual string Type { get; set; }

        public virtual UserEntity User { get; set; }

        public virtual ClientEntity Client { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual DateTime? ExpirationTime { get; set; }

        public virtual string Data { get; set; }

        public virtual bool Equals(PersistedGrantEntity other)
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
            return Equals((PersistedGrantEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}