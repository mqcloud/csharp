using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    enum OperationTypeCode {
        Default=0,
        Request=50,
        Response=51,

        GetBaseChannelsFacade = 100,

        Pong=151,

        GetEventsPublisher=201,
        GetOperationsSubscribers=201,

        SetEventsPublisher=300,
        SetOperationsSubscriber=301
    }
}