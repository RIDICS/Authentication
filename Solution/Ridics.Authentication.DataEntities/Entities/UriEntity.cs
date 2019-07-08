using System;
using System.Collections.Generic;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class UriEntity : IEquatable<UriEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Uri { get; set; }

        public virtual ISet<UriTypeEntity> UriTypes { get; set; }

        public virtual ClientEntity Client { get; set; }

        public virtual bool Equals(UriEntity other)
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
            return Equals((UriEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}