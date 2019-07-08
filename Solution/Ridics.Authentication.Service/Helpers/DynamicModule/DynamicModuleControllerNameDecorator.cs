using System;
using Ridics.Authentication.Modules.Shared;

namespace Ridics.Authentication.Service.Helpers.DynamicModule
{
    public static class DynamicModuleControllerNameDecorator
    {
        private const string ControllerPrefix = "DynamicModule";
        
        public static string GetControllerName(Type viewModelType)
        {
            var viewModel = (IModuleConfigurationViewModel) Activator.CreateInstance(viewModelType);

            return GetControllerName(viewModel);
        }

        public static string GetControllerName(IModuleConfigurationViewModel viewModel)
        {
            return $"{ControllerPrefix}{viewModel.DynamicControllerName}Controller";
        }
    }
}
