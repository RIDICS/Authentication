using System;
using Newtonsoft.Json;

namespace Ridics.Core.ReCaptcha.Models.ReCaptcha
{
    public class ReCaptchaResponse
    {
        public bool Success { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTimeStamp { get; set; }

        public string Hostname { get; set; }
    }
}