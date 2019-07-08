using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class UserDataTypeEntity : IEquatable<UserDataTypeEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string DataTypeValue { get; set; }

        public virtual ISet<UserDataEntity> UserData { get; set; }

        public virtual bool Equals(UserDataTypeEntity other)
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
            return Equals((UserDataTypeEntity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}