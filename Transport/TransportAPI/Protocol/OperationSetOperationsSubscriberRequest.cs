using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationSetOperationsSubscriberRequest : OperationRequest<OperationResponse> {
        [ProtoMember(1)]
        public string Address { get; set; }
        [ProtoMember(2)]
        public string Topic { get; set; }

        public OperationSetOperationsSubscriberRequest() {
            TypeAttributes.Add((int)OperationTypeCode.SetOperationsSubscriber);
        }
    }
}