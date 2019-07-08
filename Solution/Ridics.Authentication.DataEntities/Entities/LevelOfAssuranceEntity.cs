using System;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class LevelOfAssuranceEntity : IEquatable<LevelOfAssuranceEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual LevelOfAssuranceEnum Name { get; set; }

        public virtual int Level { get; set; }

        public virtual bool Equals(LevelOfAssuranceEntity other)
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
            return Equals((LevelOfAssuranceEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
