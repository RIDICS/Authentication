using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Ridics.Authentication.Modules.Shared
{
    public abstract class ModuleConfigurationBase<TConfig> : IModuleConfiguration
        where TConfig : IModuleConfiguration
    {
        public void Hydrate(IModuleConfiguration configuration)
        {
            if (!(configuration is TConfig))
            {
                throw new ArgumentException("Configuration can be hydrated only by instance of same type");
            }

            Hydrate((TConfig) configuration);
        }

        public abstract string GetStateHash();

        public virtual bool Enable { get; set; }
        public virtual string Name { get; set; }

        public virtual string DisplayName { get; set; }

        protected abstract void Hydrate(TConfig configuration);

        public string Serialize()
        {
            var xmlSerializer = new XmlSerializer(typeof(TConfig));

            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var stringWriter = new StringWriter())
            using (var textWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
            {
                xmlSerializer.Serialize(textWriter, this, XmlNamespaceFactory.CreateEmptyNamespace());

                return stringWriter.ToString();
            }
        }

        public virtual bool IsValid(out IList<string> errors)
        {
            var isValid = true;
            errors = new List<string>();

            if (string.IsNullOrEmpty(Name))
            {
                isValid = false;
                errors.Add($"Invalid {nameof(Name)}");
            }

            if (string.IsNullOrEmpty(DisplayName))
            {
                isValid = false;
                errors.Add($"Invalid {nameof(DisplayName)}");
            }

            return isValid;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            BeforeReadXml();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    ProcessValue(reader.Name, reader);
                }
            }
        }

        protected virtual void BeforeReadXml()
        {
        }

        protected virtual void ProcessValue(string key, XmlReader reader)
        {
            switch (key)
            {
                case nameof(Enable):
                    Enable = reader.ReadElementContentAsBoolean();

                    break;
                case nameof(Name):
                    Name = reader.ReadElementContentAsString();

                    break;
                case nameof(DisplayName):
                    DisplayName = reader.ReadElementContentAsString();

                    break;
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Enable));
            writer.WriteValue(Enable);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(Name));
            writer.WriteValue(Name);
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(DisplayName));
            writer.WriteValue(DisplayName);
            writer.WriteEndElement();
        }
    }
}
