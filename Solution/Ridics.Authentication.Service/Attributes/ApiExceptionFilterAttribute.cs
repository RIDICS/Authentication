using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Models.DataResult;
using Ridics.Authentication.DataContracts;

namespace Ridics.Authentication.Service.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Handles all unhandled errors on auth service API. Use this filter on API controllers.
    /// </summary>
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary> 
        /// Instead of returning whole error page, when error is thrown, this method creates response with status code 500 and sets the response content to <see cref="ContractException"/>,
        /// where code is set to GenericError and description is set to error message.
        /// </summary>
        /// <param name="context">Exception context</param>
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);

            var logger = context.HttpContext.RequestServices.GetService<ILogger<ApiExceptionFilterAttribute>>();
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError(context.Exception, "Unhandled exception");
            }

            var contractException = new ContractException
            {
                Code = DataResultErrorCode.GenericError,
                Description = context.Exception.Message,
            };

            context.Result = new ObjectResult(contractException) { StatusCode = 500};

            context.ExceptionHandled = true;
        }
    }
}