using System;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class TwoFactorLoginEntity : IEquatable<TwoFactorLoginEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual UserEntity User { get; set; }

        public virtual string TokenProvider { get; set; }

        public virtual string Token { get; set; }

        public virtual DateTime CreateTime { get; set; }

        public virtual bool Equals(TwoFactorLoginEntity other)
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
            return Equals((TwoFactorLoginEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}