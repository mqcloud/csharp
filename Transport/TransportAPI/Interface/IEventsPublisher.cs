namespace MQCloud.Transport.Interface {
    public interface IEventsPublisher {
        void Send(byte[] data);
    }
}