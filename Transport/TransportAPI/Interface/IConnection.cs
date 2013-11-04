namespace MQCloud.Transport.Interface {
    public delegate void Response(byte[] response);

    public delegate void OperationCallback(byte[] request, Response callback);
    public delegate void EventCallback(byte[] data);

    public interface IConnection {
        bool SubscribeToOperations(string topic, OperationCallback callback);
        bool SubscribeToEvents(string topic, EventCallback callback);

        void UnSubscribeFromOperations(string topic);
        void UnSubscribeFromEvents(string topic);

        IOperationsPublisher GetOperationsPublisher(string topic);
        IEventsPublisher GetEventsPublisher(string topic);
    }
}