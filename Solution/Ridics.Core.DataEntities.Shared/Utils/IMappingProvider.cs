using System;
using System.Collections.Generic;

namespace Ridics.Core.DataEntities.Shared.Utils
{
    public interface IMappingProvider
    {
        IEnumerable<Type> GetMappings();
    }
}