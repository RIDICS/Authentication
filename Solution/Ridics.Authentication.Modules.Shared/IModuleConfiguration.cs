using System.Collections.Generic;

namespace Ridics.Authentication.Modules.Shared
{
    /// <summary>
    /// Interface filled in registration process of the module from configuration section Modules:[]:Configuration:Configuration
    /// </summary>
    public interface IModuleConfiguration
    {
        void Hydrate(IModuleConfiguration configuration);

        string GetStateHash();

        bool Enable { get; set; }

        string Name { get; }

        string DisplayName { get; }

        string Serialize();

        bool IsValid(out IList<string> errors);
    }
}
