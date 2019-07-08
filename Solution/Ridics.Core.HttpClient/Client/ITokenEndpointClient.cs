using System.Threading.Tasks;
using IdentityModel.Client;

namespace Ridics.Core.HttpClient.Client
{
    public interface ITokenEndpointClient
    {
        Task<TokenResponse> GetAccessTokenAsync(string scope);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
        Task<TokenRevocationResponse> RevokeTokenAsync(string refreshToken);
    }
}