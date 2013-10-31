using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetEventsPublisherResponse : OperationResponse {
        [ProtoMember(1)]
        public string Address { get; set; }
    }
}