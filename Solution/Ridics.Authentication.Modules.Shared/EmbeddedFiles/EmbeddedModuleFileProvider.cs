using System.Linq;
using System.Reflection;

namespace Ridics.Authentication.Modules.Shared.EmbeddedFiles
{
    public class EmbeddedModuleFileProvider
    {
        private readonly Assembly m_assembly;

        public EmbeddedModuleFileProvider(Assembly assembly)
        {
            m_assembly = assembly;
        }

        public IModuleFile[] GetModuleFiles(string namespaceWithoutAssembly)
        {
            var assemblyNamespace = m_assembly.GetName().Name;
            var namespacePrefix = $"{assemblyNamespace}.{namespaceWithoutAssembly}";

            var embeddedFileNames = m_assembly.GetManifestResourceNames();

            var result = embeddedFileNames
                .Where(x => x.StartsWith(namespacePrefix))
                .Select(x => new EmbeddedModuleFile(m_assembly, x))
                .Cast<IModuleFile>()
                .ToArray();

            return result;
        }
    }
}
