using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    [ProtoInclude((int)OperationTypeCode.Pong, typeof(OperationPongRequest))]

    [ProtoInclude((int)OperationTypeCode.GetEventsPublisher, typeof(OperationGetEventsPublisherRequest))]
    [ProtoInclude((int)OperationTypeCode.GetOperationsPublisher, typeof(OperationGetOperationsPublisherRequest))]

    [ProtoInclude((int)OperationTypeCode.SetEventsPublisher, typeof(OperationSetEventsPublisherRequest))]
    [ProtoInclude((int)OperationTypeCode.SetOperationsPublisher, typeof(OperationSetOperationsPublisherRequest))]
    internal class OperationRequest : Operation { }

    internal class OperationRequest<TOperationResponse> : OperationRequest where TOperationResponse : OperationResponse, new() {
        public TOperationResponse GetResponse() {
            return new TOperationResponse();
        }
    }
}