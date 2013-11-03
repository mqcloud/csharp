namespace MQCloud.Transport.Implementation
{
    internal class ThematicUserOperation : ThematicMessage<byte[]> {
        public int CallbackId { get; set; }
    }
}