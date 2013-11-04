namespace MQCloud.Transport.Implementation {
    internal class ThematicOperationResponse<T> : ThematicMessage<T> {
        public int CallbackId { get; set; }
    }
}