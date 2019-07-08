using System;
using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using DryIoc.Transactions;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Connection;
using NHibernate.Mapping.ByCode;
using Ridics.Core.DataEntities.Shared.ConfigModels;

namespace Ridics.Core.DataEntities.Shared.Utils
{
    /// <summary>
    /// Configure NHibernate and it's database connection
    /// </summary>
    public abstract class NHibernateInstallerBase : INHibernateInstaller
    {
        private readonly IEnumerable<IMappingProvider> m_mappingProviders;

        protected NHibernateInstallerBase(IEnumerable<IMappingProvider> mappingProviders)
        {
            m_mappingProviders = mappingProviders;
        }

        public abstract string ConnectionString { get; }

        public abstract DatabaseServerType DatabaseServerType { get; }

        public abstract bool UseCustomConventionModelMapper { get; }

        public virtual IEnumerable<Type> GetMappingTypes()
        {
            var types = new List<Type>();

            foreach (var mappingProvider in m_mappingProviders)
            {
                types.AddRange(mappingProvider.GetMappings());
            }

            return types;
        }


        public bool IsDefault => true;

        public string SessionFactoryKey => "default";

        public Maybe<IInterceptor> Interceptor => Maybe.None<IInterceptor>();

        public Configuration Config
        {
            get
            {
                var cfg = new Configuration()
                    .DataBaseIntegration(db =>
                    {
                        db.ConfigureDialectAndDriver(DatabaseServerType);
                        db.ConnectionString = ConnectionString;
                        db.ConnectionProvider<DriverConnectionProvider>();
                        //db.LogFormattedSql = true;
                        //db.LogSqlInConsole = true;
                    });

                cfg.AddMapping(GetMappings());

                if (DatabaseServerType == DatabaseServerType.PostgreSql)
                {
                    // Workaround for creating correct queries (native NHibernate's method supports this only for keywords)
                    QuotingUtils.QuoteTableAndColumns(cfg);
                }

                return cfg;
            }
        }

        private HbmMapping GetMappings()
        {
            var mapper = UseCustomConventionModelMapper ? new CustomConventionModelMapper() : new ModelMapper();
            mapper.AddMappings(GetMappingTypes());

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            return mapping;
        }

        public void Registered(ISessionFactory factory)
        {
        }

        public Configuration Deserialize()
        {
            return null;
        }

        public void Serialize(Configuration configuration)
        {
            // Not required
        }

        public void AfterDeserialize(Configuration configuration)
        {
            // Not required
        }
    }
}