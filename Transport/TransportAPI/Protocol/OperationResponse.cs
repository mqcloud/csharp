using ProtoBuf;

namespace MQCloud.Transport.Protocol {

    [ProtoContract]
    [ProtoInclude((int)OperationTypeCode.GetEventsPublisher, typeof(OperationGetEventsPublisherResponse))]
    [ProtoInclude((int)OperationTypeCode.GetOperationsPublisher, typeof(OperationGetOperationsPublisherResponse))]
    internal class OperationResponse : Operation {
        [ProtoMember(1)]
        public OperationStatusCode State { get; set; }
        [ProtoMember(2)]
        public string Error { get; set; }
    }
}