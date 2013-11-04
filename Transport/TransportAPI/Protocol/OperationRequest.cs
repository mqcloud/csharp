using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    [ProtoInclude((int)OperationTypeCode.GetBaseChannelsFacade, typeof(OperationGetBaseChannelsFacadeRequest))]

    [ProtoInclude((int)OperationTypeCode.Pong, typeof(OperationPongRequest))]

    [ProtoInclude((int)OperationTypeCode.GetEventsPublisher, typeof(OperationGetEventsPublisherRequest))]
    [ProtoInclude((int)OperationTypeCode.GetOperationsSubscribers, typeof(OperationGetOperationsSubscribersRequest))]

    [ProtoInclude((int)OperationTypeCode.SetEventsPublisher, typeof(OperationSetEventsPublisherRequest))]
    [ProtoInclude((int)OperationTypeCode.SetOperationsSubscriber, typeof(OperationSetOperationsSubscriberRequest))]
    internal class OperationRequest : Operation {
        public OperationRequest() {
            TypeAttributes.Add((int)OperationTypeCode.Request);
        }
    }

    internal class OperationRequest<TOperationResponse> : OperationRequest where TOperationResponse : OperationResponse, new() {
        public TOperationResponse GetResponse() {
            return new TOperationResponse();
        }
    }
}