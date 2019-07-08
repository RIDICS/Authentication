namespace Ridics.Authentication.Core.Models
{
    public class UserExternalIdentityModel
    {
        public int Id { get; set; }

        public ExternalIdentityModel ExternalIdentityType { get; set; }

        public string ExternalIdentity { get; set; }

        public UserInfoModel User { get; set; }
    }
}