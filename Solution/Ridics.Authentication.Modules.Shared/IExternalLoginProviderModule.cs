namespace Ridics.Authentication.Modules.Shared
{
    /// <summary>
    /// Markup interface for external identity
    /// </summary>
    public interface IExternalLoginProviderModule
    {
        IExternalLoginProviderManager ExternalLoginProviderManager { get; }
    }
}
