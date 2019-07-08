using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ridics.Authentication.Modules.Shared
{
    public interface IContainerRegistration
    {
        void Install(
            IServiceCollection services,
            ModuleContext moduleContext,
            string hostingEnvironmentName,
            ILoggerFactory loggerFactory
        );
    }
}