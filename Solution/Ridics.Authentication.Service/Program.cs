using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Ridics.Authentication.Service.DynamicModule;
using Ridics.Authentication.Service.Utils;

namespace Ridics.Authentication.Service
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceShouldStart = true;

            while (serviceShouldStart)
            {
                var host = CreateWebHostBuilder(args).Build();

                var liveManager = host.Services.GetRequiredService<LiveManager>();

                await host.RunAsync(liveManager.GetCancellationToken());

                serviceShouldStart = liveManager.ServiceShouldRestart;
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((buildContext, configuration) =>
                {
                    var environment = buildContext.HostingEnvironment.EnvironmentName;

                    configuration.AddJsonFile("appsettings.json5", optional: false);
                    configuration.AddJsonFile($"appsettings.{environment}.json5", optional: true);

                    configuration.AddJsonFile("modules.json", optional: true);
                    configuration.AddJsonFile($"modules.{environment}.json5", optional: true);
                    configuration.AddXmlFile(DynamicModuleConfigurationManager.ModuleConfigurationFile, optional: true);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddAzureWebAppDiagnostics();
                })
                .UseNLog();
        }
    }
}
