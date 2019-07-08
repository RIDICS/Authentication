using System;
using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using Ridics.Core.DataEntities.Shared.Utils;
using Ridics.Core.Test.Shared.Database.Interceptors;
using Xunit.Abstractions;

namespace Ridics.Core.Test.Shared.Database
{
    public class MockDbFactory
    {
        private readonly IDatabaseFactory m_databaseFactory;
        private readonly IEnumerable<Type> m_mappings;
        private readonly ITestOutputHelper m_testOutputHelper;

        //Specify ITestOutputHelper, resolve it from IoContainer in test class, to enable logging of sql statements
        public MockDbFactory(IDatabaseFactory databaseFactory, IEnumerable<Type> mappings, ITestOutputHelper testOutputHelper = null)
        {
            m_databaseFactory = databaseFactory;
            m_mappings = mappings;
            m_testOutputHelper = testOutputHelper;
        }

        public ISessionManager CreateSessionManager(bool useConventionModelMapper)
        {
            var config = CreateConfig(useConventionModelMapper);
            return m_databaseFactory.CreateSessionManager(config);
        }

        private Configuration CreateConfig(bool useConventionModelMapper)
        {
            var mapper = useConventionModelMapper ? new CustomConventionModelMapper() : new ModelMapper();
            mapper.AddMappings(m_mappings);
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var fluentConfig = Fluently.Configure().Database(m_databaseFactory.Configure());

            var config = fluentConfig.BuildConfiguration();
            config.AddDeserializedMapping(mapping, null);

            //If ITestOutputHelper is set create XUnitSqlCaptureInterceptor that logs sql statements
            if (m_testOutputHelper != null)
            {
                config.SetInterceptor(new XUnitSqlCaptureInterceptor(m_testOutputHelper));
            }
            
            return config;
        }
    }
}