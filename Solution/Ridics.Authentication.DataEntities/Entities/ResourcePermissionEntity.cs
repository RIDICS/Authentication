using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ResourcePermissionEntity : IEquatable<ResourcePermissionEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string ResourceId { get; set; }

        public virtual ResourcePermissionTypeActionEntity ResourceTypeAction { get; set; }

        public virtual ISet<UserEntity> Users { get; set; }

        public virtual ISet<RoleEntity> Roles { get; set; }


        public virtual bool Equals(ResourcePermissionEntity other)
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
            return Equals((ResourcePermissionEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}