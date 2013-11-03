using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    [ProtoInclude((int)OperationTypeCode.Request, typeof(OperationRequest))]
    [ProtoInclude((int)OperationTypeCode.Response, typeof(OperationResponse))]
    internal class Operation : Message {
        public Operation() {
            TypeAttributes.Add((int)MessageTypeCode.Operation);
        }
    }
}