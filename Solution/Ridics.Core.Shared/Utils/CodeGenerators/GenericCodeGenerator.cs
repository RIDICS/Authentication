using System;
using System.Text;

namespace Ridics.Core.Shared.Utils.CodeGenerators
{
    public class GenericCodeGenerator : IGenericCodeGenerator
    {
        private static readonly object m_lock = new object();

        private readonly Random m_random = new Random();

        public string GenerateCode(int codeLength, char[] allowedChars)
        {
            var sb = new StringBuilder(codeLength);

            lock (m_lock)
            {
                for (var i = 0; i < codeLength; i++)
                {
                    sb.Append(allowedChars[m_random.Next(allowedChars.Length)]);
                }
            }
            
            return sb.ToString();
        }
    }
}