using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridics.Authentication.Service.Authorization;

namespace Ridics.Authentication.Service.Controllers
{
    [Authorize(Policy = PolicyNames.ViewAuthServiceAdministrationPolicy)]
    public class HomeController : AuthControllerBase<HomeController>
    {
        public IActionResult Index()
        {
           return View();
        }
    }
}