using System;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class UserContactEntity : IEquatable<UserContactEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Value { get; set; }

        public virtual ContactTypeEnum Type { get; set; }

        public virtual string ConfirmCode { get; set; }

        public virtual DateTime ActiveFrom { get; set; }

        public virtual DateTime? ActiveTo { get; set; }

        public virtual DateTime? ConfirmCodeChangeTime { get; set; }

        public virtual UserEntity User { get; set; }

        public virtual LevelOfAssuranceEntity LevelOfAssurance { get; set; }

        public virtual DataSourceEntity DataSource { get; set; }

        public virtual bool Equals(UserContactEntity other)
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
            return Equals((UserContactEntity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
