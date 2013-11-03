namespace MQCloud.Transport.Exceptions {
    public class GatewayException : TransportException {
        public GatewayException() { }

        public GatewayException(string message)
            : base(message) {
        }
    }
}