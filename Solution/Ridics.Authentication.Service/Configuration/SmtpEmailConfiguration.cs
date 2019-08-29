namespace Ridics.Authentication.Service.Configuration
{
    public class SmtpEmailConfiguration
    {
        public string SenderAddress { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }
}
