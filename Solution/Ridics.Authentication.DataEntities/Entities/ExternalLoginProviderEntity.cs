using System;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class ExternalLoginProviderEntity : IEquatable<ExternalLoginProviderEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        public virtual bool CreateNotExistUser { get; set; }

        public virtual bool DisableManagingByUser { get; set; }

        public virtual bool HideOnLoginScreen { get; set; }

        public virtual FileResourceEntity Logo { get; set; }

        public virtual string MainColor { get; set; }

        public virtual DynamicModuleEntity DynamicModule { get; set; }

        public virtual string DescriptionLocalizationKey => $"{Name}-description";
        
        public virtual bool Equals(ExternalLoginProviderEntity other)
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
            return Equals((ExternalLoginProviderEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
