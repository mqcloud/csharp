using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationPongRequest : OperationRequest<OperationResponse> {
        [ProtoMember(1)]
        public string Id { get; set; }

        public OperationPongRequest() {
            TypeAttributes.Add((int)OperationTypeCode.Pong);
        }
    }
}