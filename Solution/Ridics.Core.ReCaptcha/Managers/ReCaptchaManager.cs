using System;
using System.Net;
using Newtonsoft.Json;
using Ridics.Core.ReCaptcha.Config;
using Ridics.Core.ReCaptcha.Models.ReCaptcha;

namespace Ridics.Core.ReCaptcha.Managers
{
    public class ReCaptchaManager
    {
        private readonly ReCaptchaConfig m_config;

        public ReCaptchaManager(ReCaptchaConfig config)
        {
            m_config = config;
        }

        public void InitRecaptchaModel(ReCaptchaModel reCaptchaModel)
        {
            if (reCaptchaModel == null)
            {
                throw new ArgumentException("Can not init empty ReCaptcha model");
            }

            reCaptchaModel.RecaptchaPublicKey = m_config.PublicKey;
        }

        /// <summary>
        /// Checks reCaptcha results.
        /// </summary>
        /// <returns></returns>
        public bool Validate(ReCaptchaModel model)
        {
            return Validate(model.RecaptchaResponse);
        }

        /// <summary>
        /// Checks reCaptcha results.
        /// </summary>
        /// <returns></returns>
        public bool Validate(string recaptchaResponse)
        {
            if (string.IsNullOrEmpty(recaptchaResponse))
            {
                return false;
            }

            var result = VerifyResponse(recaptchaResponse);

            return result.Success;
        }

        private ReCaptchaResponse VerifyResponse(string gRecaptchaResponse)
        {
            var secretKey = m_config.SecretKey;

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentException("Secret key is not configured.");
            }

            var client = new WebClient();

            var googleReply = client.DownloadString(string.Format(m_config.ResponseUrl, secretKey, gRecaptchaResponse));

            var reCaptcha = JsonConvert.DeserializeObject<ReCaptchaResponse>(googleReply);

            return reCaptcha;
        }
    }
}