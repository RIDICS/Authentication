using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Mapping.ByCode.Conformist;

namespace Ridics.Core.DataEntities.Shared.Utils
{
    public static class NHibernateMappingProvider
    {
        public static IEnumerable<Type> GetMappingsOfType(Type baseType)
        {
            var assembly = baseType.Assembly;

            return assembly.GetExportedTypes().Where(
                t => baseType.IsAssignableFrom(t)
            );
        }

        public static IEnumerable<Type> GetMappingsFromAssembly(Assembly assembly)
        {
            var baseTypes = new[] {typeof(ClassMapping<>), typeof(SubclassMapping<>), typeof(JoinedSubclassMapping<>), typeof(UnionSubclassMapping<>)};

            return assembly.GetExportedTypes().Where(
                t => t.BaseType != null &&
                     t.BaseType.IsGenericType &&
                     baseTypes.Contains(t.BaseType.GetGenericTypeDefinition())
            );
        }
    }
}
