using System.Collections.Generic;
using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetOperationsPublisherResponse : OperationResponse {
        [ProtoMember(1)]
        public List<string> Addresses { get; set; }

        public OperationGetOperationsPublisherResponse() {
            TypeAttributes.Add((int)OperationTypeCode.GetOperationsPublisher);

            Addresses=new List<string>();
        }
    }
}