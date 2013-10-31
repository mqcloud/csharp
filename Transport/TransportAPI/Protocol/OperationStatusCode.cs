using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    internal enum OperationStatusCode {
        OperationStatusCodeOk=0,
        OperationStatusCodeError=1
    }
}