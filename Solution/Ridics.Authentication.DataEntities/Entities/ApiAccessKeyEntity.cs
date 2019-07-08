using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ApiAccessKeyEntity : IEquatable<ApiAccessKeyEntity>
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string ApiKeyHash { get; set; }
        public virtual string HashAlgorithm { get; set; }
        public virtual ICollection<ApiAccessKeyPermissionEntity> Permissions { get; set; }

        public virtual bool Equals(ApiAccessKeyEntity other)
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
            return Equals((ApiAccessKeyEntity)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}