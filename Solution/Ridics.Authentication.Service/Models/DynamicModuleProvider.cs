using System;
using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Modules.Shared;

namespace Ridics.Authentication.Service.Models
{
    public class DynamicModuleProvider
    {
        private readonly IList<ModuleContext> m_moduleContexts;

        public DynamicModuleProvider(
            IList<ModuleContext> moduleContexts
        )
        {
            m_moduleContexts = moduleContexts;
        }

        public IEnumerable<ModuleContext> ModuleContexts
        {
            get => m_moduleContexts;
        }

        public IList<ILibModuleInfo> GetLibraryModuleInfos()
        {
            return ModuleContexts.Select(x => x.LibModuleInfo).ToList();
        }

        public IList<LibraryModuleConfiguration> GetLibraryModuleConfigurations()
        {
            return ModuleContexts.Select(x => x.LibraryModuleConfiguration).ToList();
        }

        public IList<IModuleConfiguration> GetModuleConfigurations()
        {
            return ModuleContexts.Select(x => x.ModuleConfiguration).Where(x => x != null).ToList();
        }

        public IList<IModuleConfiguration> GetDefaultModuleConfigurations()
        {
            return ModuleContexts.Select(x => x.DefaultModuleConfiguration).Where(x => x != null).ToList();
        }

        public ModuleContext GetContextByNameOrGuid(string name, Guid moduleGuid)
        {
            var dynamicModuleContext = ModuleContexts.FirstOrDefault(x => x.ModuleConfiguration?.Name == name)
                                       ?? ModuleContexts.FirstOrDefault(x => x.LibModuleInfo.ModuleGuid.Equals(moduleGuid));

            return dynamicModuleContext;
        }
    }
}
