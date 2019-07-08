using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ridics.Authentication.Core.Utils.Hashing;

namespace Ridics.Authentication.Core.Managers
{
    public class HashManager
    {
        public Encoding DefaultEncoding = Encoding.ASCII;

        private readonly IDictionary<string, IHasher> m_hashersDictionary;

        public HashManager(IEnumerable<IHasher> hashers)
        {
            m_hashersDictionary = hashers.ToDictionary(x => x.Name);
        }

        public string GenerateHash(string value, string algorithm, Encoding encoding = null)
        {
            encoding = encoding ?? DefaultEncoding;

            var hasher = GetHasher(algorithm);

            var hash = hasher.GenerateHash(value, encoding);

            return hash;
        }

        public bool ValidateHash(string value, string hash, string algorithm, Encoding encoding = null)
        {
            encoding = encoding ?? DefaultEncoding;

            var hasher = GetHasher(algorithm);

            var generatedHash = hasher.GenerateHash(value, encoding);

            return hash.Equals(generatedHash, StringComparison.InvariantCultureIgnoreCase);
        }

        public IList<string> GetAllAlgorithms()
        {
            return m_hashersDictionary.Keys.ToList();
        }

        private IHasher GetHasher(string algorithm)
        {
            if (!m_hashersDictionary.TryGetValue(algorithm, out var hasher))
            {
                throw new ArgumentException();
            }

            return hasher;
        }
    }
}