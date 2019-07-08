using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Utils;
using Ridics.Core.Service.Shared;
using Swashbuckle.AspNetCore.Swagger;

namespace Ridics.Authentication.Service.IoC
{
    public static class IocSwaggerRegistrationExtensions
    {
        public static void RegisterApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });
        }

        public static void RegisterSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new Info
                    {
                        Version = description.ApiVersion.ToString(),
                        Title = "User REST API",
                        Description = "REST API for accessing users' data"
                    });
                }

                c.IncludeXmlComments(ServiceUtils.GetAppXmlDocumentationPath());
                c.DescribeAllEnumsAsStrings();
                
                c.AddSecurityDefinition("apiKey", new ApiKeyScheme
                {
                    In = "header",
                    Name = RequireApiAccessTokenAttribute.ApiAccessKeyHeader,
                    Type = "apiKey",
                });
                
                c.AddSecurityDefinition("implicit-oauth2", new OAuth2Scheme
                {
                    Flow = "implicit",
                    AuthorizationUrl = "/connect/authorize",
                    TokenUrl = "/connect/token",
                    Scopes = new Dictionary<string, string> {
                        { "auth_api.Internal", "API - internal" },
                        { "auth_api.Nonce", "Nonce API - access nonce endpoint" },
                    }
                });

                c.AddSecurityDefinition("client_credentials-oauth2", new OAuth2Scheme
                {
                    Flow = "application",
                    AuthorizationUrl = "/connect/authorize",
                    TokenUrl = "/connect/token",
                    Scopes = new Dictionary<string, string> {
                        { "auth_api.Internal", "API - internal" },
                        { "auth_api.Nonce", "Nonce API - access nonce endpoint" },
                    }
                });
                
                c.OperationFilter<AuthorizeCheckOperationFilter>();
            });
        }
    }
}