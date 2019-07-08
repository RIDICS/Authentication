using System;

namespace Ridics.Core.HttpClient.Authentication.Options
{
    public class AutomaticTokenManagementOptions
    {
        public string Scheme { get; set; }
        public TimeSpan RefreshBeforeExpiration { get; set; } = TimeSpan.FromMinutes(1);
        public bool RevokeRefreshTokenOnSignout { get; set; } = true;
    }
}