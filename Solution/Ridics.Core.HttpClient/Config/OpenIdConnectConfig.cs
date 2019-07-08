using System.Collections.Generic;

namespace Ridics.Core.HttpClient.Config
{
    public class OpenIdConnectConfig
    {
        public string Url { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public IList<string> Scopes { get; set; }
    }
}