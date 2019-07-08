using DryIoc.Facilities.NHibernate;
using FluentNHibernate.Cfg.Db;
using Moq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Ridics.Core.Test.Shared.Database
{
    public class MemoryDatabaseFactory : IDatabaseFactory
    {
        public IPersistenceConfigurer Configure()
        {
            return SQLiteConfiguration.Standard.InMemory().ConnectionString("Data Source=:memory:;Version=3;New=True;DateTimeKind=Utc").ShowSql().FormatSql(); //Specify connection string to handle all datetimes as UTC
        }

        private void ExportSchema(Configuration config, ISession session)
        {
            new SchemaExport(config).Execute(true, true, false, session.Connection, null);
            session.Flush();
        }

        public ISessionManager CreateSessionManager(Configuration config)
        {
            var sessionFactory = config.BuildSessionFactory();
            var session = sessionFactory.OpenSession();
            ExportSchema(config, session);

            var sessionManagerMock = new Mock<ISessionManager>();
            sessionManagerMock.Setup(x => x.OpenSession()).Returns(session);

            return sessionManagerMock.Object;
        }
    }
}
