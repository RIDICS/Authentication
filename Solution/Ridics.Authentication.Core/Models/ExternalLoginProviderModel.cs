namespace Ridics.Authentication.Core.Models
{
    public class ExternalLoginProviderModel
    {
        #region Hydrated using IExternalLoginProviderHydrator

        public bool Enable { get; set; }

        #endregion

        public int Id { get; set; }

        public bool CreateNotExistUser { get; set; }

        public bool DisableManagingByUser { get; set; }

        public bool HideOnLoginScreen { get; set; }

        public string DisplayName { get; set; }

        public string AuthenticationScheme { get; set; }

        public FileResourceModel Logo { get; set; }

        public string MainColor { get; set; }

        public string DescriptionLocalizationKey { get; set; }

        protected bool Equals(ExternalLoginProviderModel other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExternalLoginProviderModel) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
