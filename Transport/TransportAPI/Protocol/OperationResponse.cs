using ProtoBuf;

namespace MQCloud.Transport.Protocol {

    [ProtoContract]
    [ProtoInclude((int)OperationTypeCode.GetEventsPublisher, typeof(OperationGetEventsPublisherResponse))]
    [ProtoInclude((int)OperationTypeCode.GetOperationsSubscribers, typeof(OperationGetOperationsSubscribersResponse))]
    [ProtoInclude((int)OperationTypeCode.GetBaseChannelsFacade, typeof(OperationGetBaseChannelsFacadeResponse))]
    internal class OperationResponse : Operation {
        [ProtoMember(1)]
        public OperationStatusCode State { get; set; }
        [ProtoMember(2)]
        public string Error { get; set; }

        public OperationResponse() {
            TypeAttributes.Add((int)OperationTypeCode.Response);
        }
    }
}