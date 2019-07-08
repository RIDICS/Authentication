using System;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class FileResourceEntity : IEquatable<FileResourceEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual Guid Guid { get; set; }

        /// <summary>
        /// Store type of entity, currently only support External -> meaning store on filesystem in FileResource directory
        /// </summary>
        public virtual FileResourceEnum Type { get; set; }

        /// <summary>
        /// File extension of stored resource
        /// </summary>
        public virtual string FileExtension { get; set; }

        public virtual bool Equals(FileResourceEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileResourceEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
