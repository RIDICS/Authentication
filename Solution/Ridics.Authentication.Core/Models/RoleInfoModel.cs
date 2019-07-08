namespace Ridics.Authentication.Core.Models
{
    public class RoleInfoModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool AuthenticationServiceOnly { get; set; }
    }
}