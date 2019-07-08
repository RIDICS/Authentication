using System.Collections.Generic;

namespace Ridics.Authentication.Core.Configuration
{
    public class ExternalIdentityResolveConfiguration
    {
        public Dictionary<string, IList<string>> DataTypeExternalIdentitiesDict { get; set; }
    }
}