namespace MQCloud.Transport.Implementation {
    internal class ThematicMessage<T> {
        public T Message { get; set; }
        public string Topic { get; set; }
    }
}