namespace Ridics.Authentication.Service.Helpers
{
    public static class ContentType
    {
        public const string ImageJpeg = "image/jpeg";
        public const string ImagePng = "image/png";
        public const string ImageGig = "image/gif";
        public const string ImageWebp = "image/webp";
        public const string ImageSvg = "image/svg+xml";

        public const string ApplicationJavascript = "application/javascript";

        public static string[] ImageContentTypes =
        {
            ImageJpeg,
            ImagePng,
            ImageGig,
            ImageWebp,
            ImageSvg,
        };
    }
}
