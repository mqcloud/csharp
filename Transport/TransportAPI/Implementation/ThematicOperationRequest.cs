namespace MQCloud.Transport.Implementation {
    internal class ThematicOperationRequest<T> : ThematicMessage<T> {
        public int CallbackId { get; set; }
        public string SenderId { get; set; }
    }
}