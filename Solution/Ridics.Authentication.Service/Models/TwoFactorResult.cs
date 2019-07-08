namespace Ridics.Authentication.Service.Models
{
    public class TwoFactorResult
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
    }
}