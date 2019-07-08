using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ridics.Authentication.Service.Attributes;
using Ridics.Authentication.Service.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ridics.Authentication.Service.Utils
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var jwtAuthAttribute = GetSpecificAttribute(typeof(JwtAuthorizeAttribute), context.MethodInfo);

            if (jwtAuthAttribute != null && jwtAuthAttribute is JwtAuthorizeAttribute castedJwtAuthAttribute)
            {
                operation.Responses.Add("401", new Response {Description = "Unauthorized"});
                operation.Responses.Add("403", new Response {Description = "Forbidden"});

                var scope = "auth_api";

                if (castedJwtAuthAttribute.Policy == PolicyNames.InternalApiPolicy)
                {
                    scope = "auth_api.Internal";
                }
                else if (castedJwtAuthAttribute.Policy == PolicyNames.NonceApiPolicy)
                {
                    scope = "auth_api.Nonce";
                }

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        { "implicit-oauth2", new[] { scope }},
                        { "client_credentials-oauth2", new[] { scope }},
                    }
                };

                return;
            }

            var apiAccessAttribute = GetSpecificAttribute(typeof(RequireApiAccessTokenAttribute), context.MethodInfo);

            if (apiAccessAttribute != null)
            {
                operation.Responses.Add("401", new Response { Description = "Unauthorized" });

                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>> {{ "apiKey", new string[0] }}
                };
            }
        }

        private Attribute GetSpecificAttribute(Type attributeType, MemberInfo methodInfo)
        {
            var methodAttributes = methodInfo.GetCustomAttributes();
            var controllerAttributes = methodInfo.ReflectedType.GetCustomAttributes();

            return methodAttributes.FirstOrDefault(x => x.GetType() == attributeType) ?? controllerAttributes.FirstOrDefault(x => x.GetType() == attributeType);
        }
    }
}