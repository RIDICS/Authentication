using System;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ApiAccessKeyPermissionEntity : IEquatable<ApiAccessKeyPermissionEntity>
    {
        public virtual int Id { get; protected set; }
        public virtual ApiAccessKeyEntity ApiAccessKey { get; set; }
        public virtual ApiAccessPermissionEnum Permission { get; set; }

        public virtual bool Equals(ApiAccessKeyPermissionEntity other)
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
            return Equals((ApiAccessKeyPermissionEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}