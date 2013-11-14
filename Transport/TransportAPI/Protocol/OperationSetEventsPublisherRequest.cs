using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationSetEventsPublisherRequest : OperationRequest<OperationResponse> {
        [ProtoMember(1)]
        public string Topic { get; set; }

        public OperationSetEventsPublisherRequest() {
            TypeAttributes.Add((int)OperationTypeCode.SetEventsPublisher);
        }
    }
}