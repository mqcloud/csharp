using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetOperationsPublisherResponse : OperationResponse {
        [ProtoMember(1)]
        public string Address { get; set; }
    }
}