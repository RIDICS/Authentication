using System;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Ridics.Authentication.Modules.Shared
{
    public class ModuleContext
    {
        private readonly IModuleConfiguration m_moduleConfiguration;

        public ModuleContext(
            Assembly assembly,
            Type moduleConfigType,
            LibraryModuleConfiguration libraryModuleConfiguration,
            IModuleConfigurationManager moduleConfigurationManager,
            EmbeddedFileProvider embeddedFileProvider,
            IModuleConfiguration moduleConfiguration,
            IModuleConfiguration defaultModuleConfiguration
        )
        {
            Assembly = assembly;
            ModuleConfigType = moduleConfigType;
            LibraryModuleConfiguration = libraryModuleConfiguration;
            ModuleConfigurationManager = moduleConfigurationManager;
            EmbeddedFileProvider = embeddedFileProvider;
            m_moduleConfiguration = moduleConfiguration;
            DefaultModuleConfiguration = defaultModuleConfiguration;
        }

        public Assembly Assembly { get; }

        public Type ModuleConfigType { get; }

        public LibraryModuleConfiguration LibraryModuleConfiguration { get; }

        public IModuleConfigurationManager ModuleConfigurationManager { get; }

        public EmbeddedFileProvider EmbeddedFileProvider { get; }

        public IModuleConfiguration ModuleConfiguration => m_moduleConfiguration ?? DefaultModuleConfiguration;

        public IModuleConfiguration DefaultModuleConfiguration { get; }

        public IModuleConfiguration EmptyModuleConfiguration => Activator.CreateInstance(ModuleConfigType) as IModuleConfiguration;

        public ILibModuleInfo LibModuleInfo { get; set; }
    }
}
