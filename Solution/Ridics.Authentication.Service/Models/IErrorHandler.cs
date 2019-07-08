using Microsoft.AspNetCore.Mvc;

namespace Ridics.Authentication.Service.Models
{
    public interface IErrorHandler
    {
        IActionResult GetErrorView(int statusCode, string message, string messageDetail);
        IActionResult GetErrorView(int statusCode);
    }
}
