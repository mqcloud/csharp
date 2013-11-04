using System.Collections.Generic;
using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetEventsPublisherResponse : OperationResponse {
        [ProtoMember(1)]
        public List<string> Addresses { get; set; }

        public OperationGetEventsPublisherResponse() {
            Addresses = new List<string>();
            TypeAttributes.Add((int)OperationTypeCode.GetEventsPublisher);
        }
    }
}