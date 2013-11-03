using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetEventsPublisherRequest : OperationRequest<OperationGetEventsPublisherResponse> {
        [ProtoMember(1)]
        public string Topic { get; set; }

        public OperationGetEventsPublisherRequest() {
            TypeAttributes.Add((int)OperationTypeCode.GetEventsPublisher);
        }
    }
}