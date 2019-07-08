using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Ridics.Authentication.Service.Extensions;
using Ridics.Core.DataEntities.Shared.ConfigModels;
using Ridics.Core.DataEntities.Shared.Utils;

namespace Ridics.Authentication.Service.IoC
{
    public class NHibernateInstaller : NHibernateInstallerBase
    {
        private readonly IConfiguration m_configuration;

        public NHibernateInstaller(IConfiguration configuration, IEnumerable<IMappingProvider> mappingProviders) : base(mappingProviders)
        {
            m_configuration = configuration;
        }

        public override string ConnectionString => m_configuration.GetConnectionString("MainDatabase");

        public override DatabaseServerType DatabaseServerType => m_configuration.GetDatabaseServerType();

        public override bool UseCustomConventionModelMapper => false;
    }
}