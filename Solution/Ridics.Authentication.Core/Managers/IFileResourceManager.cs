using System.IO;
using Ridics.Authentication.Core.Models;

namespace Ridics.Authentication.Core.Managers
{
    public interface IFileResourceManager
    {
        string ResolveFullPath(FileResourceModel fileResource);

        string ResolveName(FileResourceModel fileResource);

        FileStream GetReadStream(FileResourceModel fileResource);

        FileStream GetWriteStream(FileResourceModel fileResource);
    }
}
