using System;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Ridics.Authentication.Modules.Shared;

namespace Ridics.Authentication.Service.Models.DynamicModule
{
    public class ModuleContextCandidate
    {
        public ModuleContextCandidate(
            Assembly assembly,
            Type moduleConfigType,
            LibraryModuleConfiguration libraryModuleConfiguration,
            IModuleConfigurationManager moduleConfigurationManager,
            EmbeddedFileProvider embeddedFileProvider,
            IModuleConfiguration defaultModuleConfiguration
        )
        {
            Assembly = assembly;
            ModuleConfigType = moduleConfigType;
            LibraryModuleConfiguration = libraryModuleConfiguration;
            ModuleConfigurationManager = moduleConfigurationManager;
            EmbeddedFileProvider = embeddedFileProvider;
            DefaultModuleConfiguration = defaultModuleConfiguration;
        }

        public Assembly Assembly { get; }

        public Type ModuleConfigType { get; }

        public LibraryModuleConfiguration LibraryModuleConfiguration { get; }

        public IModuleConfigurationManager ModuleConfigurationManager { get; }

        public EmbeddedFileProvider EmbeddedFileProvider { get; }

        public IModuleConfiguration DefaultModuleConfiguration { get; }
    }
}
