using System.IO;

namespace Ridics.Authentication.Modules.Shared
{
    /// <summary>
    /// Represents file and access to it's content
    /// </summary>
    public interface IModuleFile
    {
        /// <summary>
        /// File name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get stream for reading file
        /// </summary>
        /// <returns></returns>
        Stream FileStream { get; }
    }
}
