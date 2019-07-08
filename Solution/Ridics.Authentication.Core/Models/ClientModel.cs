using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ClientModel : ClientInfoModel
    {
        public IList<SecretModel> Secrets { get; set; }

        public IList<GrantTypeModel> AllowedGrantTypes { get; set; }

        public IList<UriModel> UriList { get; set; }

        public IList<IdentityResourceModel> AllowedIdentityResources { get; set; }

        public IList<ScopeModel> AllowedScopes { get; set; }
    }
}