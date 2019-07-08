using System;

namespace Ridics.Authentication.Modules.Shared
{
    public abstract class ModuleConfigurationManagerBase<TConfig, TViewModel> : IModuleConfigurationManager
        where TConfig : IModuleConfiguration
        where TViewModel : IModuleConfigurationViewModel
    {
        public Type ViewModelType { get; } = typeof(TViewModel);

        public BaseModuleConfigurationViewModel CreateViewModel(IModuleConfiguration configuration, IModuleLocalization localization)
        {
            if (!(configuration is TConfig))
            {
                throw new InvalidOperationException(
                    $"ViewModel can be created only from type {typeof(TConfig)}"
                );
            }

            return CreateViewModel((TConfig) configuration, localization);
        }

        public void HydrateModuleConfiguration(IModuleConfiguration configuration, IModuleConfigurationViewModel viewModel)
        {
            if (!(configuration is TConfig))
            {
                throw new InvalidOperationException(
                    $"Only type {typeof(TConfig)} can be hydrated"
                );
            }

            if (!(viewModel is TViewModel))
            {
                throw new InvalidOperationException(
                    $"Only type {typeof(TViewModel)} can hydrate configuration"
                );
            }

            HydrateModuleConfiguration((TConfig) configuration, (TViewModel) viewModel);
        }

        protected abstract BaseModuleConfigurationViewModel CreateViewModel(TConfig configuration, IModuleLocalization localization);

        protected abstract void HydrateModuleConfiguration(TConfig configuration, TViewModel viewModel);
    }
}
