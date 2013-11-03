using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetBaseChannelsFacadeResponse : OperationResponse {
        [ProtoMember(1)]
        public string OperationsAddress { get; set; }

        [ProtoMember(2)]
        public string EventsAddress { get; set; }

        public OperationGetBaseChannelsFacadeResponse() {
            TypeAttributes.Add((int)OperationTypeCode.GetBaseChannelsFacade);
        }
    }
}