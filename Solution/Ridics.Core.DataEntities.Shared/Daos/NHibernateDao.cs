using System;
using System.Collections;
using System.Collections.Generic;
using DryIoc.Facilities.NHibernate;
using NHibernate;
using Ridics.Core.Shared.Exceptions;

namespace Ridics.Core.DataEntities.Shared.Daos
{
    public class NHibernateDao
    {
        private readonly ISessionManager m_sessionManager;
        public const string WildcardAny = "%";
        public const string WildcardSingle = "_";

        public NHibernateDao(ISessionManager sessionManager)
        {
            m_sessionManager = sessionManager;
        }

        protected ISession GetSession()
        {
            return m_sessionManager.OpenSession();
        }

        public static string EscapeQuery(string query)
        {
            return query?.Replace("[", "[[]");
        }

        public virtual object FindById(Type type, object id)
        {
            try
            {
                return GetSession().Get(type, id);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Get by id operation failed for type:{type.Name}", ex);
            }
        }

        public virtual T FindById<T>(object id)
        {
            try
            {
                return (T) GetSession().Get(typeof(T), id);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Get by id operation failed for type:{typeof(T).Name}", ex);
            }
        }

        public virtual object Load(Type type, object id)
        {
            try
            {
                return GetSession().Load(type, id);
            }
            catch (ObjectNotFoundException)
            {
                throw;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Load by id operation failed for type:{type.Name}", ex);
            }
        }

        public virtual T Load<T>(object id)
        {
            try
            {
                return (T) GetSession().Load(typeof(T), id);
            }
            catch (ObjectNotFoundException)
            {
                throw;
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Load by id operation failed for type:{typeof(T).Name}", ex);
            }
        }

        public virtual object Create(object instance)
        {
            try
            {
                return GetSession().Save(instance);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Create operation failed for type:{instance.GetType().Name}", ex);
            }
        }

        public virtual IList<object> CreateAll(IEnumerable data)
        {
            var result = new List<object>();
            foreach (var instance in data)
            {
                try
                {
                    var id = GetSession().Save(instance);
                    result.Add(id);
                }
                catch (HibernateException ex)
                {
                    throw new DatabaseException($"Create operation failed for type:{instance.GetType().Name}", ex);
                }
            }

            return result;
        }

        public virtual void Delete(object instance)
        {
            try
            {
                var session = GetSession();
                session.Delete(instance);
                session.Flush(); //force flush to detect possible exceptions
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Delete operation failed for type:{instance.GetType().Name}", ex);
            }
        }

        public virtual void Update(object instance)
        {
            try
            {
                GetSession().Update(instance);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Update operation failed for type:{instance.GetType().Name}", ex);
            }
        }

        public virtual void DeleteAll(Type type)
        {
            try
            {
                GetSession().Delete($"from {type.Name}");
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Delete operation failed for type:{type.Name}", ex);
            }
        }

        public virtual void DeleteAll(IEnumerable data)
        {
            foreach (var o in data)
            {
                Delete(o);
            }
        }

        public virtual void Save(object instance)
        {
            try
            {
                GetSession().SaveOrUpdate(instance);
            }
            catch (HibernateException ex)
            {
                throw new DatabaseException($"Save or Update operation failed for type:{instance.GetType().Name} ", ex);
            }
        }

        public virtual void SaveAll(IEnumerable data)
        {
            foreach (var o in data)
            {
                Save(o);
            }
        }
    }
}