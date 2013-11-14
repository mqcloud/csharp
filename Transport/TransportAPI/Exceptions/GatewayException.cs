using System;

namespace MQCloud.Transport.Exceptions {
    [Serializable]
    public class GatewayException : TransportException {
        public GatewayException() { }

        public GatewayException(string message)
            : base(message) {
        }
    }
}