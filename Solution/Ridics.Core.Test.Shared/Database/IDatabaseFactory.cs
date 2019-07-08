using DryIoc.Facilities.NHibernate;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;

namespace Ridics.Core.Test.Shared.Database
{
    public interface IDatabaseFactory
    {
        IPersistenceConfigurer Configure();

        ISessionManager CreateSessionManager(Configuration config);
    }
}