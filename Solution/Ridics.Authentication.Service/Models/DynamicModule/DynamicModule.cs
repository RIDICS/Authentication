using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Ridics.Authentication.Modules.Shared;
using Ridics.Authentication.Service.Utils;

namespace Ridics.Authentication.Service.Models.DynamicModule
{
    public class DynamicModule : IXmlSerializable
    {
        public string Checksum { get; set; }

        public DateTime LastConfigurationReload { get; set; }

        public List<DynamicModuleInstanceConfiguration> Configuration { get; set; }

        public DynamicModule()
        {
            Configuration = new List<DynamicModuleInstanceConfiguration>();
        }

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
            writer.WriteStartElement("Checksum");
            writer.WriteValue(Checksum);
            writer.WriteEndElement();

            writer.WriteStartElement("LastConfigurationReload");
            writer.WriteValue(LastConfigurationReload);
            writer.WriteEndElement();

            var xmlSerializer = new XmlSerializer(typeof(DynamicModuleInstanceConfiguration));

            foreach (var configuration in Configuration)
            {
                xmlSerializer.Serialize(writer, configuration, XmlNamespaceFactory.CreateEmptyNamespace());
            }
        }
    }
}
