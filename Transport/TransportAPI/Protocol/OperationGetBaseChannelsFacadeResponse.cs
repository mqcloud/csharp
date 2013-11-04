using System.Collections.Generic;
using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetBaseChannelsFacadeResponse : OperationResponse {
        [ProtoMember(1)]
        public string UserId { get; set; }

        [ProtoMember(2)]
        public string OperationsAddress { get; set; }

        [ProtoMember(3)]
        public string EventsAddress { get; set; }

        [ProtoMember(4)]
        public List<string> EventTopics { get; set; }

        public OperationGetBaseChannelsFacadeResponse() {
            EventTopics = new List<string>();
            TypeAttributes.Add((int)OperationTypeCode.GetBaseChannelsFacade);
        }
    }
}