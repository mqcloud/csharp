using System.Collections.Generic;
using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class EventPeers : Event {
        [ProtoMember(1)]
        public List<string> Added { get; set; }

        [ProtoMember(2)]
        public List<string> Removed { get; set; }

        public EventPeers() {
            TypeAttributes.Add((int)EventTypeCode.Peers);

            Added=new List<string>();
            Removed=new List<string>();
        }
    }
}