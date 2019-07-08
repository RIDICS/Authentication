using System;

namespace Ridics.Authentication.Core.Models
{
    public class SecretModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }

        public DateTime? Expiration { get; set; }
    }
}