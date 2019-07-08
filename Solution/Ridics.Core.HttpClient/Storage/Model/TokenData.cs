using System;

namespace Ridics.Core.HttpClient.Storage.Model
{
    public class TokenData
    {
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}