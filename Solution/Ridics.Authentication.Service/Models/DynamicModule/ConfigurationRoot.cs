using System;
using System.Xml.Serialization;

namespace Ridics.Authentication.Service.Models.DynamicModule
{
    [Serializable()]
    [XmlRoot("configuration")]
    public class ConfigurationRoot
    {
        [XmlElement("Modules")]
        public DynamicModule Module { get; set; }

        public ConfigurationRoot()
        {
            Module = new DynamicModule();
        }
    }
}