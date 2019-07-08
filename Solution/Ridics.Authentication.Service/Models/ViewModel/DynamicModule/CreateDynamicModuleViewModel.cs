using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Modules.Shared;

namespace Ridics.Authentication.Service.Models.ViewModel.DynamicModule
{
    public class CreateDynamicModuleViewModel
    {
        public const string CustomName = "__custom__";

        [Display(Name = "name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [DataType(DataType.Text)] 
        public string NameOption { get; set; }

        [Display(Name = "module-type")]
        [DataType(DataType.Text)]
        public Guid ModuleGuid { get; set; }

        [Display(Name = "version")]
        [DataType(DataType.Text)]
        public Version ConfigurationVersion { get; set; }

        public IList<ILibModuleInfo> DynamicModules { get; set; }

        public IList<string> AvailableDynamicModule { get; set; }
    }
}