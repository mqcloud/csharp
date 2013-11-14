using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationSetOperationsSubscriberRequest : OperationRequest<OperationResponse> {
        [ProtoMember(1)]
        public string Topic { get; set; }

        [ProtoMember(2)]
        public bool Active { get; set; }

        public OperationSetOperationsSubscriberRequest() {
            TypeAttributes.Add((int)OperationTypeCode.SetOperationsSubscriber);
        }
    }
}