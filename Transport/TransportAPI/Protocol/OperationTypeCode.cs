namespace MQCloud.Transport.Protocol {
    enum OperationTypeCode {
        Default=0,
        Request=1,
        Response=2,

        Pong=10,

        GetEventsPublisher=100,
        GetOperationsPublisher=101,

        SetEventsPublisher=200,
        SetOperationsPublisher=201
    }
}