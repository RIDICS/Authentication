﻿using System;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ExternalIdentityEntity : IEquatable<ExternalIdentityEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual bool Equals(ExternalIdentityEntity other)
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
            return Equals((ExternalIdentityEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}