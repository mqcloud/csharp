using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    enum EventTypeCode { }
    [ProtoContract]
    [ProtoInclude((int)MessageTypeCode.Operation, typeof(Operation))]
    internal class Event : Message { }
}