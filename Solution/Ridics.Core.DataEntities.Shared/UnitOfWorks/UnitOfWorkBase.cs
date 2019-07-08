using DryIoc.Facilities.NHibernate;
using NHibernate;

namespace Ridics.Core.DataEntities.Shared.UnitOfWorks
{
    public abstract class UnitOfWorkBase
    {
        private readonly ISessionManager m_sessionManager;

        protected UnitOfWorkBase(ISessionManager sessionManager)
        {
            m_sessionManager = sessionManager;
        }

        protected ISession GetSession()
        {
            return m_sessionManager.OpenSession();
        }
    }
}