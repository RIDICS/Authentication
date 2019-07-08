using System;
using System.Security.Cryptography;

namespace Ridics.Core.Shared.Utils.CodeGenerators
{
    /// <summary>
    /// The code generator using RNGCryptoServiceProvider. The bias is negligible for small allowed character sets.
    /// Source: https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings/13416143#13416143
    /// </summary>
    public class CryptographyCodeGenerator : IGenericCodeGenerator, IDisposable
    {
        private readonly RNGCryptoServiceProvider m_rngCryptoServiceProvider = new RNGCryptoServiceProvider();

        public string GenerateCode(int codeLength, char[] allowedChars)
        {
            var bytes = new byte[codeLength * 8];
            m_rngCryptoServiceProvider.GetBytes(bytes);

            var result = new char[codeLength];
            for (int i = 0; i < codeLength; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = allowedChars[value % (uint) allowedChars.Length];
            }
            return new string(result);
        }

        public void Dispose()
        {
            m_rngCryptoServiceProvider?.Dispose();
        }
    }
}