using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    enum MessageTypeCode {
        Default=0,
        Operation=1,
        Event=2,
    }

    [ProtoContract]
    [ProtoInclude((int)MessageTypeCode.Operation, typeof(Operation))]
    [ProtoInclude((int)MessageTypeCode.Event, typeof(Event))]
    internal class Message { }
}