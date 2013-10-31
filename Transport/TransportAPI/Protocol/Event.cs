using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    enum EventTypeCode {
        Default=0,
        Ping=1,
        Peers=2
    }

    [ProtoContract]
    [ProtoInclude((int)EventTypeCode.Ping, typeof(EventPingRequest))]
    [ProtoInclude((int)EventTypeCode.Peers, typeof(EventPeers))]
    internal class Event : Message { }
}