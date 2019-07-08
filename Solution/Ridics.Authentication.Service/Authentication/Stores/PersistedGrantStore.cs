using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Service.Authentication.Stores
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly PersistedGrantManager m_persistedGrantManager;

        public PersistedGrantStore(PersistedGrantManager persistedGrantManager)
        {
            m_persistedGrantManager = persistedGrantManager;
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            var result = m_persistedGrantManager.SavePersistedGrant(Mapper.Map<PersistedGrantModel>(grant));
            //TODO investigate action if not success
            return Task.CompletedTask;
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            var result = m_persistedGrantManager.FindPersistedGrantByKey(key);

            return result.HasError ? null : Task.FromResult(Mapper.Map<PersistedGrant>(result.Result));
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var result = m_persistedGrantManager.GetAllPersitedGrantForUser(subjectId);

            return result.HasError ? null : Task.FromResult(Mapper.Map<IList<PersistedGrant>>(result.Result).AsEnumerable());
        }

        public Task RemoveAsync(string key)
        {
            var result = m_persistedGrantManager.DeleteGrantByKey(key); //TODO investigate action if not success

            return Task.CompletedTask;
        }

        public Task RemoveAllAsync(string subjectId, string clientId)
        {
            var result = m_persistedGrantManager.DeleteAllPersistedGrant(subjectId, clientId); //TODO investigate action if not success

            return Task.CompletedTask;
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var result = m_persistedGrantManager.DeleteAllPersistedGrant(subjectId, clientId,
                type); //TODO investigate action if not success

            return Task.CompletedTask;
        }
    }
}