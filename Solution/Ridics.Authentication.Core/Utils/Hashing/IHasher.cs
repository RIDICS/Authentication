using System.Text;

namespace Ridics.Authentication.Core.Utils.Hashing
{
    public interface IHasher
    {
        string GenerateHash(string input, Encoding encoding);
        string Name { get; }
    }
}