using System;
using Ridics.Authentication.DataEntities.Entities.Enums;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class DataSourceEntity : IEquatable<DataSourceEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual DataSourceEnum DataSource { get; set; }

        public virtual ExternalLoginProviderEntity ExternalLoginProvider { get; set; }

        public virtual bool Equals(DataSourceEntity other)
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
            return Equals((DataSourceEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
