﻿using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ResourceEntity : IEquatable<ResourceEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ISet<ClaimTypeEntity> ClaimTypes { get; set; }

        public virtual bool Required { get; set; }

        public virtual bool ShowInDiscoveryDocument { get; set; }

        public virtual bool Equals(ResourceEntity other)
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
            return Equals((ResourceEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}