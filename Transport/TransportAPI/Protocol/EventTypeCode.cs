using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    enum EventTypeCode {
        Default=0,
        Ping=100,
        Peers=200
    }
}