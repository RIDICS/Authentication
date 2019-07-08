using NHibernate;
using NHibernate.SqlCommand;
using Xunit.Abstractions;

namespace Ridics.Core.Test.Shared.Database.Interceptors
{
    public class XUnitSqlCaptureInterceptor : EmptyInterceptor
    {
        private readonly ITestOutputHelper m_testOutputHelper;

        public XUnitSqlCaptureInterceptor(ITestOutputHelper testOutputHelper)
        {
            m_testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Writes prepared sql statement with ITestOutputHelper
        /// </summary>
        /// <param name="sql">Sql statement to write</param>
        /// <returns><paramref name="sql"/></returns>
        public override SqlString OnPrepareStatement(SqlString sql)
        {
            m_testOutputHelper.WriteLine(sql.ToString());
            return sql;
        }


    }
}