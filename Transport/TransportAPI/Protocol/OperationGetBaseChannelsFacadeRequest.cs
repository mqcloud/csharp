using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetBaseChannelsFacadeRequest : OperationRequest<OperationGetBaseChannelsFacadeResponse> {

        [ProtoMember(1)]
        public string ApplicationName { get; set; }

        public OperationGetBaseChannelsFacadeRequest() {
            TypeAttributes.Add( (int)OperationTypeCode.GetBaseChannelsFacade );
        }
    }
}