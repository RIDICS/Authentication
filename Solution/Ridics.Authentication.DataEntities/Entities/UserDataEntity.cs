using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class UserDataEntity : IEquatable<UserDataEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual UserEntity User { get; set; }

        public virtual UserEntity VerifiedBy { get; set; }

        public virtual UserDataTypeEntity UserDataType { get; set; }

        public virtual UserDataEntity ParentUserData { get; set; }

        public virtual IList<UserDataEntity> ChildrenUserData { get; set; }

        public virtual string Value { get; set; }
        
        public virtual DateTime ActiveFrom { get; set; }

        public virtual DateTime? ActiveTo { get; set; }

        public virtual LevelOfAssuranceEntity LevelOfAssurance { get; set; }

        public virtual DataSourceEntity DataSource { get; set; }

        public virtual bool Equals(UserDataEntity other)
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
            return Equals((UserDataEntity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
