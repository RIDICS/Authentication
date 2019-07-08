using System;
using System.Collections.Generic;
using Ridics.Authentication.DataEntities.Mappings;
using Ridics.Core.DataEntities.Shared.Utils;

namespace Ridics.Authentication.DataEntities.Providers
{
    public class AuthorizationMappingProvider : IMappingProvider
    {
        public IEnumerable<Type> GetMappings()
        {
            return NHibernateMappingProvider.GetMappingsOfType(typeof(IMapping));
        }
    }
}
