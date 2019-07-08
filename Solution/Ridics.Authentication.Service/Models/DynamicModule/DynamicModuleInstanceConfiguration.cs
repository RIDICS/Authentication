using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ridics.Authentication.Modules.Shared;

namespace Ridics.Authentication.Service.Models.DynamicModule
{
    [XmlRoot("Configuration")]
    public class DynamicModuleInstanceConfiguration : IXmlSerializable
    {
        public int Name { get; set; }

        public string AssemblyName { get; set; }

        public IModuleConfiguration Configuration { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(
                "Name",
                Name.ToString()
            );

            writer.WriteStartElement("AssemblyName");
            writer.WriteValue(AssemblyName);
            writer.WriteEndElement();

            if (Configuration != null)
            {
                var xmlSerializer = new XmlSerializer(Configuration.GetType());

                xmlSerializer.Serialize(writer, Configuration, XmlNamespaceFactory.CreateEmptyNamespace());
            }
        }
    }
}
