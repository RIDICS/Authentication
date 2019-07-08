using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.Models.ViewModel.Account
{
    public class ExternalLoginProviderViewModel
    {
        public int Id { get; set; }

        public bool Enable { get; set; }

        public string DisplayName { get; set; }

        public bool DisableManagingByUser { get; set; }

        public string AuthenticationScheme { get; set; }

        public FileResourceModel Logo { get; set; }

        public string LogoFileName { get; set; }

        public string MainColor { get; set; }

        protected bool Equals(ExternalLoginProviderViewModel other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ExternalLoginProviderViewModel) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
