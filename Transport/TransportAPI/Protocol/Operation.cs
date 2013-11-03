using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    [ProtoInclude((int)OperationTypeCode.Request, typeof(OperationRequest))]
    [ProtoInclude((int)OperationTypeCode.Response, typeof(OperationResponse))]
    internal class Operation : Message {

        // Warning: Side that receives OperationRequest shall not bound its logic to CallbackId!
        [ProtoMember(1)]        
        public int CallbackId { get; set; } 

        public Operation() {
            TypeAttributes.Add((int)MessageTypeCode.Operation);
        }
    }
}