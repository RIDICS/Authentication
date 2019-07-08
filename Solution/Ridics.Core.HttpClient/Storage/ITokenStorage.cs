using System;
using Ridics.Core.HttpClient.Storage.Model;

namespace Ridics.Core.HttpClient.Storage
{
    public interface ITokenStorage
    {
        TokenData GetAccessToken();
        void StoreAccessToken(string accessToken, DateTime accessTokenExpiration);
    }
}