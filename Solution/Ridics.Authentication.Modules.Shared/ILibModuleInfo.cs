using System;
using System.Reflection;

namespace Ridics.Authentication.Modules.Shared
{
    /// <summary>
    /// Basic info required for loading module. Every library module has to have one class implementing this interface.
    /// This interface implementations should not be registered in IoC by module.
    /// </summary>
    public interface ILibModuleInfo : IEquatable<ILibModuleInfo>
    {
        /// <summary>
        /// Unique module identifier
        /// </summary>
        /// <value></value>
        Guid ModuleGuid { get; }

        /// <summary>
        /// Version of library
        /// </summary>
        /// <value></value>
        Version Version { get; }

        /// <summary>
        /// Default module name displayed to user
        /// </summary>
        /// <value></value>
        string DefaultDisplayName { get; }

        string ConfigureComponentName { get; }

        IModuleFile[] GetLocalizationFiles(Assembly assembly);

        /// <summary>
        /// Class containing all registrations to IoC container
        /// </summary>
        /// <returns></returns>
        IContainerRegistration ContainerRegistration { get; }
    }
}
