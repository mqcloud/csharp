using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetOperationsSubscribersRequest : OperationRequest<OperationGetOperationsSubscribersResponse> {
        [ProtoMember(1)]
        public string Topic { get; set; }

        public OperationGetOperationsSubscribersRequest() {
            TypeAttributes.Add((int)OperationTypeCode.GetOperationsSubscribers);
        }
    }
}