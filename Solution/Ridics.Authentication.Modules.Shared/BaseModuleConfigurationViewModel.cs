using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Modules.Shared
{
    /// <summary>
    /// This is configuration counter part to <see cref="IModuleConfiguration"/>
    /// </summary>
    public abstract class BaseModuleConfigurationViewModel : IModuleConfigurationViewModel
    {
        public abstract string DynamicControllerName { get; }

        public string ParentPropertyName { get; set; }

        [Display(Name = "enable")]
        public bool Enable { get; set; }

        public string Name { get; set; }

        [Display(Name = "display-name")]
        public string DisplayName { get; set; }

        public virtual bool HasEditableName { get; set; } = false;
    }
}
