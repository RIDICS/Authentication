using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ResourcePermissionTypeActionEntity : IEquatable<ResourcePermissionTypeActionEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ResourcePermissionTypeEntity ResourcePermissionType { get; set; }

        public virtual ISet<ResourcePermissionEntity> ResourcePermissions { get; set; }

        public virtual ISet<UserEntity> Users { get; set; }

        public virtual ISet<RoleEntity> Roles { get; set; }

        public virtual bool Equals(ResourcePermissionTypeActionEntity other)
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
            return Equals((ResourcePermissionTypeActionEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}