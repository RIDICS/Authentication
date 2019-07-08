using System;
using NHibernate.Mapping.ByCode;

namespace Ridics.Core.DataEntities.Shared.Utils
{
    public class CustomConventionModelMapper : ModelMapper
    {
        public const string TableClassNameSuffix = "Entity";
        public const string ForeignKeyColumnSuffix = "Id";

        public CustomConventionModelMapper()
        {
            // Remove "Entity" suffix from class name
            BeforeMapClass += (inspector, type, classCustomizer) =>
                classCustomizer.Table($"`{GetTableName(type.Name)}`");

            // Add "Id" suffix for mapping property to foreign key
            BeforeMapManyToOne += (inspector, member, customizer) =>
                customizer.Column($"`{member.LocalMember.Name}{ForeignKeyColumnSuffix}`");
        }

        private string GetTableName(string typeName)
        {
            var lastIndexOfSuffix = typeName.LastIndexOf(TableClassNameSuffix, StringComparison.Ordinal);
            return lastIndexOfSuffix > 0 ? typeName.Substring(0, lastIndexOfSuffix) : typeName;
        }
    }
}