using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ApiAccessKeyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ApiKeyHash { get; set; }
        public string HashAlgorithm { get; set; }
        public IList<ApiAccessKeyPermissionModel> Permissions { get; set; }
    }
}