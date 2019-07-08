using System;

namespace Ridics.Core.DataEntities.Shared.Entities
{
    public abstract class EntityBase<TEntity, TId> : IEquatable<TEntity> where TId : struct where TEntity : EntityBase<TEntity, TId>
    {
        public virtual TId Id { get; set; }

        public virtual bool Equals(TEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}