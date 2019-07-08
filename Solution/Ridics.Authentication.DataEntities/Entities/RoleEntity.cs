using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class RoleEntity : IEquatable<RoleEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ISet<UserEntity> User { get; set; }

        public virtual ISet<ResourcePermissionEntity> ResourcePermissions { get; set; }

        public virtual ISet<ResourcePermissionTypeActionEntity> ResourcePermissionTypeActions { get; set; }

        public virtual ISet<PermissionEntity> Permissions { get; set; }

        public virtual bool AuthenticationServiceOnly { get; set; }

        public virtual bool Equals(RoleEntity other)
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
            return Equals((RoleEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}