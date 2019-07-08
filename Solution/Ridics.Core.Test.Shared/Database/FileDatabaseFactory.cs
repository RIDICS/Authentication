using DryIoc.Facilities.NHibernate;
using FluentNHibernate.Cfg.Db;
using Moq;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Ridics.Core.Test.Shared.Database
{
    public class FileDatabaseFactory : IDatabaseFactory
    {
        private readonly string m_databaseFileName = "test.db";

        public FileDatabaseFactory()
        {
        }

        public FileDatabaseFactory(string fileName)
        {
            m_databaseFileName = fileName;
        }

        public IPersistenceConfigurer Configure()
        {
            return SQLiteConfiguration.Standard.ConnectionString(string.Format("Data Source={0};Version=3;New=True;DateTimeKind=Utc", m_databaseFileName)).ShowSql().FormatSql(); //Specify connection string to handle all datetimes as UTC
        }

        private void ExportSchema(Configuration config)
        {
            new SchemaExport(config).Execute(true, true, false);
        }

        public ISessionManager CreateSessionManager(Configuration config)
        {
            var sessionFactory = config.BuildSessionFactory();
            ExportSchema(config);

            var sessionManagerMock = new Mock<ISessionManager>();
            sessionManagerMock.Setup(x => x.OpenSession()).Returns(sessionFactory.OpenSession());

            return sessionManagerMock.Object;
        }
    }
}