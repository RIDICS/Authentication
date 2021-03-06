﻿using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ridics.Core.Shared.Providers;
using Ridics.Core.HttpClient.Client;
using Ridics.Core.HttpClient.Config;
using Ridics.Core.HttpClient.Storage;

namespace Ridics.Core.HttpClient.Provider
{
    public class AuthApiAccessTokenProvider
    {
        private const string AccessTokenName = "access_token";
        private const double AccessTokenExpirationWindowInMinutes = 5; 

        private readonly IHttpContextAccessor m_httpContextAccessor;
        private readonly ILogger m_logger;

        private readonly IDateTimeProvider m_dateTimeProvider;
        private readonly Func<ITokenEndpointClient> m_tokenClientFactory;
        private readonly OpenIdConnectConfig m_openIdConnectConfig;

        private readonly ITokenStorage m_tokenStorage;

        public AuthApiAccessTokenProvider(IHttpContextAccessor httpContextAccessor, 
            ILogger logger,
            Func<ITokenEndpointClient> tokenClientFactory, 
            IDateTimeProvider dateTimeProvider, 
            ITokenStorage tokenStorage, 
            OpenIdConnectConfig openIdConnectConfig)
        {
            m_httpContextAccessor = httpContextAccessor;
            m_logger = logger;
            m_tokenClientFactory = tokenClientFactory;
            m_dateTimeProvider = dateTimeProvider;
            m_tokenStorage = tokenStorage;
            m_openIdConnectConfig = openIdConnectConfig;
        }

        /// <summary>
        /// Get access token for communication with auth service api.
        /// First tries to get user access token from http context, if null gets client access token 
        /// </summary>
        /// <returns>Access token issued to user or client</returns>
        public async Task<string> GetAccessTokenAsync()
        {
            string accessToken = null;

            if (m_httpContextAccessor.HttpContext != null)
            {
                accessToken = await m_httpContextAccessor.HttpContext.GetTokenAsync(AccessTokenName);
            }
            
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = await GetClientAccessToken();
            }
            return accessToken;
        }

        private async Task<string> GetClientAccessToken()
        {
            var clientAccessToken = m_tokenStorage.GetAccessToken();

            if (clientAccessToken != null && !string.IsNullOrEmpty(clientAccessToken.Token) && clientAccessToken.TokenExpiration.AddMinutes(AccessTokenExpirationWindowInMinutes) > DateTime.UtcNow)
            {
                return clientAccessToken.Token;
            }

            var tokenResponse = await m_tokenClientFactory.Invoke().GetAccessTokenAsync(string.Join(" ", m_openIdConnectConfig.Scopes));

            if (tokenResponse.IsError)
            {
                m_logger.LogError(tokenResponse.Error);
                return string.Empty;
            }

            SaveAccessToken(tokenResponse);

            return tokenResponse.AccessToken;
        }

        private void SaveAccessToken(TokenResponse tokenResponse)
        {
            var clientAccessTokenExpiration = m_dateTimeProvider.UtcNow + TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
            m_tokenStorage.StoreAccessToken(tokenResponse.AccessToken, clientAccessTokenExpiration);
        }
    }
}