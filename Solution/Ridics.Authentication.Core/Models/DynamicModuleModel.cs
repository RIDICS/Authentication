using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Ridics.Authentication.Modules.Shared;

namespace Ridics.Authentication.Core.Models
{
    public class DynamicModuleModel
    {
        public int Id { get; protected set; }

        public Guid ModuleGuid { get; set; }

        public string Name { get; set; }

        public Version ConfigurationVersion { get; set; }

        public string ConfigurationString { get; set; }

        private IModuleConfiguration m_configuration { get; set; }

        public string ConfigurationChecksum { get; set; }

        public ISet<DynamicModuleBlobModel> DynamicModuleBlobs { get; set; }

        public IModuleConfiguration Configuration(Type moduleConfigType)
        {
            if (m_configuration != null && moduleConfigType == m_configuration.GetType())
            {
                return m_configuration;
            }

            var xmlConfigurationDeserializer = new XmlSerializer(moduleConfigType);

            using (var configurationStream = new MemoryStream(Encoding.UTF8.GetBytes(ConfigurationString)))
            {
                m_configuration = xmlConfigurationDeserializer.Deserialize(configurationStream) as IModuleConfiguration;
            }

            return m_configuration;
        }

        public string SerializeState()
        {
            var nestedState = new StringBuilder();

            foreach (var dynamicModuleBlob in DynamicModuleBlobs.OrderBy(x => x.Id))
            {
                nestedState.Append(
                    dynamicModuleBlob.SerializeState()
                );
            }

            return string.Format(
                "{0}:{1}:{2}:{3}:{4}:{5}",
                Id,
                ModuleGuid,
                Name,
                ConfigurationVersion,
                ConfigurationChecksum,
                nestedState
            );
        }
    }
}
