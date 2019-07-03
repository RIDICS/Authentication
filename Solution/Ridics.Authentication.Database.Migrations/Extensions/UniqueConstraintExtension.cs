using Dapper;
using FluentMigrator;
using Ridics.DatabaseMigrator.Shared;

namespace Ridics.Authentication.Database.Migrations.Extensions
{
    public static class UniqueConstraintExtension
    {
        public static void CreateUniqueConstraint(this MigrationBase migrationBase,
            string constraintName,
            string tableName,
            string columnName)
        {
            migrationBase.IfDatabase(DatabaseType.SqlServer).Execute.WithConnection((con, tran) =>
            {
                con.Execute(string.Format(@"CREATE UNIQUE INDEX ""{0}"" ON ""{1}"" (""{2}"") WHERE ""{3}"" IS NOT NULL;",
                        constraintName, tableName, columnName, columnName),
                    null, tran);
            });

            migrationBase.IfDatabase(DatabaseType.Postgres).Create.UniqueConstraint(constraintName).OnTable(tableName)
                .Column(columnName);
        }
    }
}