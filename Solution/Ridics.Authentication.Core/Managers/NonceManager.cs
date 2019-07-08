using System;
using System.Security.Cryptography;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.DataContracts;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Core.Managers
{
    public sealed class NonceManager : ManagerBase
    {
        private const int DefaultHashLength = 32;
        private const int DefaultNonceTimeoutInSeconds = 60;

        private readonly int m_hashLength;
        private readonly int m_nonceTimeoutInSeconds;
        private readonly IMemoryCache m_memoryCache;

        public NonceManager(
            IMemoryCache memoryCache,
            ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration
        ) : this(DefaultHashLength, DefaultNonceTimeoutInSeconds, memoryCache, logger, translator, mapper, paginationConfiguration)
        {
        }

        public NonceManager(
            int hashLength,
            int nonceTimeoutInSeconds,
            IMemoryCache memoryCache,
            ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration
        ) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_hashLength = hashLength;
            m_nonceTimeoutInSeconds = nonceTimeoutInSeconds;
            m_memoryCache = memoryCache;
        }

        public void HydrateNonceContract(NonceContract nonceContract)
        {
            var key = GenerateHash(m_hashLength);
            nonceContract.Nonce = key;

            m_memoryCache.Set(key, nonceContract, TimeSpan.FromSeconds(m_nonceTimeoutInSeconds));
        }

        public bool IsNonceKeyValid(string nonce, int userId, NonceTypeEnum nonceType)
        {
            if (nonce == null)
            {
                throw new ArgumentException();
            }

            if (m_memoryCache.TryGetValue(nonce, out NonceContract nonceContract))
            {
                var isValid = nonceContract.UserId == userId
                              && nonceContract.Type == nonceType;

                m_memoryCache.Remove(nonce);

                return isValid;
            }

            return false;
        }

        public bool IsNonceKeyValid(string nonce, NonceTypeEnum nonceType)
        {
            return IsNonceKeyValid(nonce, 0, nonceType);
        }

        private string GenerateHash(int keyLength)
        {
            var randomBytes = new byte[keyLength];

            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rngCryptoServiceProvider.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes);
        }
    }
}
