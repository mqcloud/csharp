using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class EventPingRequest : Event {
        public EventPingRequest() {
            TypeAttributes.Add((int)EventTypeCode.Ping);
        }
    }
}