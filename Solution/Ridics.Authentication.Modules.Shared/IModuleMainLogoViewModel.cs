using Microsoft.AspNetCore.Http;

namespace Ridics.Authentication.Modules.Shared
{
    public interface IModuleMainLogoViewModel
    {
        IFormFile MainLogo { get; }

        string MainLogoFileName { get; set; }
    }
}
