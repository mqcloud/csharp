using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MQCloud.Transport.Service.Implementation {
    [Serializable]
    [XmlRoot("configuration")]
    internal class Configuration {
        [XmlAttribute("host")]
        [DefaultValue("")]
        public string Host { get; set; }

        [DefaultValue(5919)]
        [XmlAttribute("port")]
        public int Port { get; set; }

        [XmlElement("ping")]
        public PingSettings PingSettings { get; set; }
    }
}