using System;

namespace Ridics.Authentication.Modules.Shared
{
    public interface IModuleConfigurationManager
    {
        Type ViewModelType { get; }

        BaseModuleConfigurationViewModel CreateViewModel(
            IModuleConfiguration configuration,
            IModuleLocalization localization
        );

        void HydrateModuleConfiguration(
            IModuleConfiguration configuration,
            IModuleConfigurationViewModel viewModel
        );
    }
}
