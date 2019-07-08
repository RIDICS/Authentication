namespace Ridics.Authentication.Modules.Shared
{
    public interface IModuleConfigurationViewModel
    {
        /// <summary>
        /// Key used as part of the name for dynamically created controller
        /// </summary>
        string DynamicControllerName { get; }

        /// <summary>
        /// Key used as fix to enable nested view model from partial view
        /// </summary>
        string ParentPropertyName { get; set; }

        bool Enable { get; set; }

        string Name { get; set; }

        bool HasEditableName { get; set; }
    }
}
