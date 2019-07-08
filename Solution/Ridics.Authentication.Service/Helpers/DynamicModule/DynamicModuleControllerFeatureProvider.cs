using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Ridics.Authentication.Modules.Shared;
using Ridics.Authentication.Service.Controllers;

namespace Ridics.Authentication.Service.Helpers.DynamicModule
{
    public class DynamicModuleControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {

        private readonly IList<ModuleContext> m_moduleContexts;

        public DynamicModuleControllerFeatureProvider(
            IList<ModuleContext> moduleContexts
        )
        {
            m_moduleContexts = moduleContexts;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var dynamicModuleGenericController = typeof(GenericDynamicModuleController<>);
            foreach (var moduleContext in m_moduleContexts)
            {
                var typeName = DynamicModuleControllerNameDecorator.GetControllerName(moduleContext.ModuleConfigurationManager.ViewModelType);

                if (feature.Controllers.All(t => t.Name != typeName))
                {
                    feature.Controllers.Add(
                        dynamicModuleGenericController
                            .MakeGenericType(moduleContext.ModuleConfigurationManager.ViewModelType)
                            .GetTypeInfo()
                    );
                }
            }
        }
    }
}
