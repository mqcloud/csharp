using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationPongRequest : OperationRequest<OperationResponse> {
        public OperationPongRequest() {
            TypeAttributes.Add((int)OperationTypeCode.Pong);
        }
    }
}