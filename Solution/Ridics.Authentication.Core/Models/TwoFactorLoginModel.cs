using System;

namespace Ridics.Authentication.Core.Models
{
    public class TwoFactorLoginModel
    {
        public int Id { get; set; }

        public string TokenProvider { get; set; }

        public string Token { get; set; }

        public DateTime CreateTime { get; set; }
    }
}