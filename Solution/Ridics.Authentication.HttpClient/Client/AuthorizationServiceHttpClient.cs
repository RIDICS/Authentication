﻿using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.HttpClient.Configuration;
using Ridics.Authentication.HttpClient.Contract;
using Ridics.Authentication.HttpClient.Exceptions;
using Ridics.Authentication.HttpClient.Provider;
using Ridics.Core.HttpClient.Client;
using Ridics.Core.HttpClient.Provider;

namespace Ridics.Authentication.HttpClient.Client
{
    public class AuthorizationServiceHttpClient : ServiceHttpClientBase<ContractException, AuthServiceException, AuthServiceApiException>
    {
        private const string CultureHeader = "X-Culture";
        private const string ApiAccessTokenHeader = "X-Api-Access-Key";

        private readonly AuthServiceControllerBasePathsProvider m_basePathsProvider;
        private readonly IAuthorizationServiceClientLocalization m_localization;
        private readonly AuthApiAccessTokenProvider m_authApiAccessTokenProvider;

        public AuthorizationServiceHttpClient(
            AuthServiceCommunicationConfiguration interServiceCommunicationConfiguration,
            AuthServiceControllerBasePathsProvider basePathsProvider,
            IAuthorizationServiceClientLocalization localization,
            ILogger logger, 
            AuthApiAccessTokenProvider authApiAccessTokenProvider
        ) : base(interServiceCommunicationConfiguration.AuthenticationServiceAddress, logger)
        {
            m_basePathsProvider = basePathsProvider;
            m_localization = localization;
            m_authApiAccessTokenProvider = authApiAccessTokenProvider;

            DefaultRequestHeaders.Add(interServiceCommunicationConfiguration.TokenName ?? ApiAccessTokenHeader,
                interServiceCommunicationConfiguration.ApiAccessToken);
        }

        protected override string ServiceName => "authorization";
        
        protected override AuthServiceException CreateServiceException(string errorMessage, Exception innerException = null)
        {
            return new AuthServiceException(errorMessage, innerException);
        }

        protected override AuthServiceApiException CreateServiceApiException(AuthServiceException serviceException,
            ContractException contractException)
        {
            var authServiceApiException = new AuthServiceApiException(serviceException.Message)
            {
                Code = contractException.Code,
                Description = contractException.Description,
                Content = serviceException.Content,
                ContentType = serviceException.ContentType,
                StatusCode = serviceException.StatusCode,
            };

            m_localization.LocalizeApiException(authServiceApiException);

            return authServiceApiException;
        }

        protected override bool IsContractExceptionValid(ContractException ex)
        {
            return ex != null && !string.IsNullOrEmpty(ex.Code) && !string.IsNullOrEmpty(ex.Description);
        }

        protected override async Task InsertCustomHeadersAsync(HttpRequestMessage request)
        {
            request.Headers.Add(CultureHeader, m_localization.GetCurrentCulture());

            var accessToken = await m_authApiAccessTokenProvider.GetAccessTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        private string GetControllerBasePath<TContract>()
        {
            return m_basePathsProvider.GetControllerBasePath<TContract>();
        }

        public virtual NameValueCollection CreateQueryCollection()
        {
            return CreateQuery();
        }

        public Task<TContract> GetItemStreamAsync<TContract>(string controllerBasePath, int id) where TContract : StreamContract, new()
        {
            return GetItemStreamAsync<TContract, StreamHydrator<TContract>>(controllerBasePath, id);
        }

        public async Task<TContract> GetItemStreamAsync<TContract, THydrator>(string controllerBasePath, int id)
            where TContract : StreamContract, new()
            where THydrator : StreamHydrator<TContract>, new()
        {
            var fullPath = $"{controllerBasePath}{id}";

            var response = await SendRequestAsync(HttpMethod.Get, fullPath);

            var responseObject = new TContract
            {
                Stream = await response.Content.ReadAsStreamAsync()
            };

            var hydrator = new THydrator();
            hydrator.Hydrate(responseObject, response);

            return responseObject;
        }

        public async Task<ListContract<TContract>> GetListAsync<TContract>(int start, int count, string search = null, NameValueCollection parameters = null)
        {
            var path = $"{GetControllerBasePath<TContract>()}list?{CreateListedQuery(start, count, search, parameters)}";
            var response = await SendRequestAsync<ListContract<TContract>>(HttpMethod.Get, path);
            return response;
        }

        public async Task<TContract> GetItemAsync<TContract>(int id, NameValueCollection parameters = null)
        {
            var path = $"{GetControllerBasePath<TContract>()}{id}";
            if (parameters != null)
            {
                var queryParameters = CreateQuery();
                queryParameters.Add(parameters);
                path = $"{path}?{queryParameters}";
            }
            var response = await SendRequestAsync<TContract>(HttpMethod.Get, path);
            return response;
        }

        public async Task<HttpResponseMessage> CreateItemAsync<TContract>(TContract content) where TContract : ContractBase
        {
            var path = $"{GetControllerBasePath<TContract>()}create";
            return await SendRequestAsync(HttpMethod.Post, path, content);
        }

        public async Task<HttpResponseMessage> EditItemAsync<TContract>(int id, TContract content) where TContract : ContractBase
        {
            var fullPath = $"{GetControllerBasePath<TContract>()}{id}/edit";
            return await SendRequestAsync(HttpMethod.Put, fullPath, content);
        }

        public async Task<HttpResponseMessage> DeleteItemAsync<TContract>(int id)
        {
            var fullPath = $"{GetControllerBasePath<TContract>()}{id}/delete";
            return await SendRequestAsync(HttpMethod.Delete, fullPath);
        }
    }
}