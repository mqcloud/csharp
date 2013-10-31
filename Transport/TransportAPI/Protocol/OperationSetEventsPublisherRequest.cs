using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationSetEventsPublisherRequest : OperationRequest<OperationResponse> {
        [ProtoMember(1)]
        public string Address { get; set; }
        [ProtoMember(2)]
        public string Topic { get; set; }
    }
}