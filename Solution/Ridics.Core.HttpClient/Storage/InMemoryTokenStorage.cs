using System;
using Ridics.Core.HttpClient.Storage.Model;

namespace Ridics.Core.HttpClient.Storage
{
    public class InMemoryTokenStorage : ITokenStorage
    {
        private TokenData m_accessToken;

        public TokenData GetAccessToken()
        {
            return m_accessToken;
        }

        public void StoreAccessToken(string accessToken, DateTime accessTokenExpiration)
        {
            m_accessToken = new TokenData{ Token = accessToken, TokenExpiration = accessTokenExpiration};
        }
    }
}