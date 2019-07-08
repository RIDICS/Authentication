using Microsoft.AspNetCore.Mvc;

namespace Ridics.Core.ReCaptcha.Models.ReCaptcha
{
    public class ReCaptchaModel
    {
        public string RecaptchaPublicKey { get; set; }

        [ModelBinder(Name = "g-recaptcha-response")]
        public string RecaptchaResponse { get; set; }
    }
}
