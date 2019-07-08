namespace Ridics.Core.Structures.Shared
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorMessageDetail { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ReturnUrl { get; set; }
    }
}