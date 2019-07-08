using System;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class UserExternalIdentityEntity : IEquatable<UserExternalIdentityEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual ExternalIdentityEntity ExternalIdentityType { get; set; }
        
        public virtual UserEntity User { get; set; }
        
        public virtual string ExternalIdentity { get; set; }

        public virtual bool Equals(UserExternalIdentityEntity other)
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
            return Equals((UserExternalIdentityEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}