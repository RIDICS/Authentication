using System.Text;
using Ridics.Core.Shared.Utils;

namespace Ridics.Authentication.Core.Utils.Hashing
{
    public class Sha256Hasher: IHasher
    {
        public string GenerateHash(string input, Encoding encoding)
        {
            return HashUtils.ConvertToSha256HashString(input, encoding);
        }

        public string Name => HashNames.Sha256;
    }
}