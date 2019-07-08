using System.Collections.Generic;
using Ridics.Authentication.Core.Managers;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Core.ExternalIdentity
{
    public class ExternalIdentityResolver : IExternalIdentityResolver
    {
        private readonly IList<ExternalIdentityModel> m_externalIdentities;
        private readonly IList<IConcreteExternalIdentityResolver> m_resolvers;

        public ExternalIdentityResolver(ExternalIdentityManager externalIdentityManager,
            ConcreteExternalIdentityResolverFactory concreteExternalIdentityResolverFactory)
        {
            m_resolvers = concreteExternalIdentityResolverFactory.CreateResolvers();
            m_externalIdentities = externalIdentityManager.FindAllExternalIdentity().Result;
        }

        public IList<UserExternalIdentityModel> Resolve(UserModel user)
        {
            var externalIdentities = new List<UserExternalIdentityModel>();

            foreach (var resolver in m_resolvers)
            {
                var resolvedExternalIdentities = resolver.Resolve(user.UserData, m_externalIdentities);
                externalIdentities.AddRange(resolvedExternalIdentities);
            }

            return externalIdentities;
        }
    }
}