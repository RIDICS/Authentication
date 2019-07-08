using System;
using System.Reflection;
using Ridics.Authentication.Modules.Shared.EmbeddedFiles;

namespace Ridics.Authentication.Modules.Shared
{
    public abstract class LibModuleInfoBase : ILibModuleInfo
    {
        public abstract Guid ModuleGuid { get; }

        public abstract Version Version { get; }

        public abstract string DefaultDisplayName { get; }

        public abstract string ConfigureComponentName { get; }

        public abstract IContainerRegistration ContainerRegistration { get; }

        protected virtual string LocalizationNamespace => "Localization";

        public IModuleFile[] GetLocalizationFiles(Assembly assembly)
        {
            var embeddedModuleFileProvider = new EmbeddedModuleFileProvider(assembly);
            var files = embeddedModuleFileProvider.GetModuleFiles(LocalizationNamespace);

            return files;
        }

        public bool Equals(ILibModuleInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ModuleGuid == other.ModuleGuid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LibModuleInfoBase) obj);
        }

        public override int GetHashCode()
        {
            return ModuleGuid.GetHashCode();
        }
    }
}
