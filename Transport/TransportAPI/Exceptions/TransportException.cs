using System;

namespace MQCloud.Transport.Exceptions {
    [Serializable]
    public class TransportException : Exception {
        public TransportException() { }
        public TransportException(string message) : base(message) { }
    }
}