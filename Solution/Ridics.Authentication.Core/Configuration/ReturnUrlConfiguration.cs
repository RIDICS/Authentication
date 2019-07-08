namespace Ridics.Authentication.Core.Configuration
{
    public class ReturnUrlConfiguration
    {
        public string DefaultRedirectUrl { get; set; }

        public string DefaultFailedExternalLoginUrl { get; set; }

        public string LoginUrlForClient { get; set; }

        public string RegisterUrlForClient { get; set; }
    }
}
