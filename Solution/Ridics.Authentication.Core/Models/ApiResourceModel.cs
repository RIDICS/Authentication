using System.Collections.Generic;

namespace Ridics.Authentication.Core.Models
{
    public class ApiResourceModel : ResourceModel
    {
        public IList<SecretModel> ApiSecrets { get; set; }

        public IList<ScopeModel> Scopes { get; set; }
    }
}