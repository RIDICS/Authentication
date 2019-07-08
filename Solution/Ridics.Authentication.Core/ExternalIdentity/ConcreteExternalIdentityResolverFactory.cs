using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;

namespace Ridics.Authentication.Core.ExternalIdentity
{
    public class ConcreteExternalIdentityResolverFactory
    {
        private readonly ExternalIdentityResolveConfiguration m_configuration;
        private readonly ILogger<ConcreteExternalIdentityResolver> m_resolverLogger;

        public ConcreteExternalIdentityResolverFactory(ExternalIdentityResolveConfiguration configuration,
            ILogger<ConcreteExternalIdentityResolver> resolverLogger)
        {
            m_configuration = configuration;
            m_resolverLogger = resolverLogger;
        }

        public IList<IConcreteExternalIdentityResolver> CreateResolvers()
        {
            var resolvers = new List<IConcreteExternalIdentityResolver>();

            if (m_configuration.DataTypeExternalIdentitiesDict == null)
            {
                return resolvers;
            }

            foreach (var userDataTypeName in m_configuration.DataTypeExternalIdentitiesDict.Keys)
            {
                var externalIdentitiesNames = m_configuration.DataTypeExternalIdentitiesDict[userDataTypeName];
                var concreteResolver = new ConcreteExternalIdentityResolver(userDataTypeName, externalIdentitiesNames, m_resolverLogger);
                resolvers.Add(concreteResolver);
            }

            return resolvers;
        }
    }
}