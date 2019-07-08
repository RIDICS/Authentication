using System;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Modules.Shared;

namespace Ridics.Authentication.Service.Models.ViewModel.DynamicModule
{
    public class DynamicModuleViewModel : DynamicModuleViewModel<BaseModuleConfigurationViewModel>
    {
    }

    public class DynamicModuleViewModel<TConfiguration> : IDynamicModuleViewModel
        where TConfiguration : class, IModuleConfigurationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Display(Name = "module-identifier")]
        [DataType(DataType.Text)]
        public Guid ModuleGuid { get; set; }

        [Display(Name = "module-type")]
        [DataType(DataType.Text)]
        public string ModuleName { get; set; }

        [Display(Name = "version")]
        [DataType(DataType.Text)]
        public Version ConfigurationVersion { get; set; }

        public string CustomConfigurationPartialName { get; set; }

        public TConfiguration CustomConfigurationViewModel { get; set; }
    }

    public interface IDynamicModuleViewModel
    {
        int Id { get; set; }

        string Name { get; set; }

        Guid ModuleGuid { get; set; }

        string ModuleName { get; set; }

        Version ConfigurationVersion { get; set; }

        string CustomConfigurationPartialName { get; set; }
    }
}
