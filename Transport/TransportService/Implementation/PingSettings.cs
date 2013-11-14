using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace MQCloud.Transport.Service.Implementation {
    [Serializable]
    internal class PingSettings {
        [DefaultValue(10000)]
        [XmlAttribute("interval")]
        public int Interval { get; set; }

        [DefaultValue(10000)]
        [XmlAttribute("policy")]
        public int Policy { get; set; }
    }
}