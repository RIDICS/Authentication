using System;
using System.Data;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace Ridics.Authentication.DataEntities.DataTypes
{
    public class VersionType : IUserType
    {
        public new bool Equals(object x, object y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var xV = (Version) x;
            var yV = (Version) y;

            return xV.Equals(yV);
        }

        public int GetHashCode(object x)
        {
            var xV = (Version) x;

            return xV.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var obj = NHibernateUtil.String.NullSafeGet(rs, names[0], session);
            if (obj == null) return null;
            var valueInString = (string) obj;
            return new Version(valueInString);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                ((IDataParameter) cmd.Parameters[index]).Value = DBNull.Value;
            }
            else
            {
                ((IDataParameter) cmd.Parameters[index]).Value = ((Version) value).ToString();
            }
        }

        public object DeepCopy(object value)
        {
            return ((Version) value).Clone();
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return new Version((string) cached);
        }

        public object Disassemble(object value)
        {
            return ((Version) value).ToString();
        }

        public SqlType[] SqlTypes => new[] {NHibernateUtil.String.SqlType};

        public Type ReturnedType => typeof(Version);

        public bool IsMutable => false;
    }
}