using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    enum MessageTypeCode {
        Default=0,
        Operation=100,
        Event=200,
    }
}