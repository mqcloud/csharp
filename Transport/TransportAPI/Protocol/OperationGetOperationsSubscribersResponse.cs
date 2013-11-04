using System.Collections.Generic;
using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal class OperationGetOperationsSubscribersResponse : OperationResponse {
        [ProtoMember(1)]
        public List<string> Addresses { get; set; }

        public OperationGetOperationsSubscribersResponse() {
            TypeAttributes.Add((int)OperationTypeCode.GetOperationsSubscribers);

            Addresses=new List<string>();
        }
    }
}