using System.Xml.Serialization;

namespace Ridics.Authentication.Modules.Shared
{
    public static class XmlNamespaceFactory
    {
        public static XmlSerializerNamespaces CreateEmptyNamespace()
        {
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, "");

            return xmlns;
        }
    }
}
