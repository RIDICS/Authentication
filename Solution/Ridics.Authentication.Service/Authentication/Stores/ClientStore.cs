using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Ridics.Authentication.Core.Managers;

namespace Ridics.Authentication.Service.Authentication.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly ClientManager m_clientManager;

        public ClientStore(ClientManager clientManager)
        {
            m_clientManager = clientManager;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var clientResult = m_clientManager.FindClientByClientId(clientId);

            var client = Mapper.Map<Client>(clientResult.Result);

            return Task.FromResult(client);
        }
    }
}