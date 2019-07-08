using NHibernate.Cfg;
using NHibernate.Mapping;

namespace Ridics.Core.DataEntities.Shared.Utils
{
    public class QuotingUtils
    {
        public static void QuoteTableAndColumns(Configuration configuration)
        {
            foreach (var cm in configuration.ClassMappings)
            {
                QuoteTable(cm.Table);
            }

            foreach (var cm in configuration.CollectionMappings)
            {
                QuoteTable(cm.Table);
            }
        }

        private static void QuoteTable(Table table)
        {
            if (!table.IsQuoted)
            {
                table.IsQuoted = true;
            }

            foreach (var column in table.ColumnIterator)
            {
                if (!column.IsQuoted)
                {
                    column.IsQuoted = true;
                }
            }
        }
    }
}
