using System.IO;
using System.Reflection;

namespace Ridics.Authentication.Modules.Shared.EmbeddedFiles
{
    public class EmbeddedModuleFile : IModuleFile
    {
        private readonly Assembly m_assembly;

        public EmbeddedModuleFile(Assembly assembly, string fullFileName)
        {
            m_assembly = assembly;
            Name = fullFileName;
        }

        public string Name { get; }

        public Stream FileStream => m_assembly.GetManifestResourceStream(Name);
    }
}
