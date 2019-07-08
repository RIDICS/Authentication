using System;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class DynamicModuleBlobEntity : IEquatable<DynamicModuleBlobEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual DynamicModuleEntity DynamicModule { get; set; }

        public virtual DynamicModuleBlobEnum Type { get; set; }

        public virtual FileResourceEntity File { get; set; }

        public virtual DateTime LastChange { get; set; }

        public virtual string SerializeState()
        {
            return string.Format(
                "{0}:{1}:{2}:{3}",
                Type.ToString(),
                File.Guid.ToString(),
                File.Type.ToString(),
                File.FileExtension
            );
        }

        public virtual bool Equals(DynamicModuleBlobEntity other)
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
            return Equals((DynamicModuleBlobEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
