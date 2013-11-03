using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    [ProtoInclude((int)EventTypeCode.Ping, typeof(EventPingRequest))]
    [ProtoInclude((int)EventTypeCode.Peers, typeof(EventPeers))]
    internal class Event : Message {
        public Event() {
            TypeAttributes.Add((int)MessageTypeCode.Event);
        }
    }
}