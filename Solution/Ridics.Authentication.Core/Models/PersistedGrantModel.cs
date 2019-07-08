using System;

namespace Ridics.Authentication.Core.Models
{
    public class PersistedGrantModel
    {
        public int Id { get; protected set; }

        public string Key { get; set; }

        public string Type { get; set; }

        public UserInfoModel User { get; set; }

        public ClientInfoModel Client { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime? ExpirationTime { get; set; }

        public virtual string Data { get; set; }
    }
}