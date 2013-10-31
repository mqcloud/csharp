using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetOperationsPublisherRequest : OperationRequest<OperationGetOperationsPublisherResponse> {
        [ProtoMember(1)]
        public string Topic { get; set; }
    }
}