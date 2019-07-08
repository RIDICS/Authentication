using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Ridics.Authentication.Service.Controllers;

namespace Ridics.Authentication.Service.Helpers.DynamicModule
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DynamicModuleControllerNameAttribute : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericDynamicModuleController<>))
            {
                var entityType = controller.ControllerType.GenericTypeArguments[0];

                controller.ControllerName = DynamicModuleControllerNameDecorator.GetControllerName(entityType);
            }
        }
    }
}
