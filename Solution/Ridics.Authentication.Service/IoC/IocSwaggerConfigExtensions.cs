using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocSwaggerConfigExtensions
    {
        public static void ConfigureSwagger(this IApplicationBuilder applicationBuilder, IApiVersionDescriptionProvider provider)
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI(c =>
            {
                var swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";

                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/{description.GroupName}/swagger.json",
                        $"User REST API {description.GroupName.ToUpperInvariant()}");
                }

                c.OAuthConfigObject = new OAuthConfigObject
                {
                    ClientId = string.Empty,
                    ClientSecret = string.Empty,
                };
            });
        }
    }
}
