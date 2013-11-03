using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetBaseChannelsFacadeRequest : OperationRequest<OperationGetBaseChannelsFacadeResponse> {
        public OperationGetBaseChannelsFacadeRequest() {
            TypeAttributes.Add((int)OperationTypeCode.GetBaseChannelsFacade);
        }
    }
}