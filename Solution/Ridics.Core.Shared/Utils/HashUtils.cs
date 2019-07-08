using System;
using System.Security.Cryptography;
using System.Text;

namespace Ridics.Core.Shared.Utils
{
    public static class HashUtils
    {
        public static string ConvertToSha256HashString(string input)
        {
            return ConvertToSha256HashString(input, Encoding.ASCII);
        }

        public static string ConvertToSha256HashString(string input, Encoding encoding)
        {
            using (var algorithm = SHA256.Create())
            {
                var hashedBytes = algorithm.ComputeHash(encoding.GetBytes(input));
                return BitConverter.ToString(hashedBytes).Replace("-", string.Empty).ToLower();
            }
        }
    }
}