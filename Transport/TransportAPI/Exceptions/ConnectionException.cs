using System;

namespace MQCloud.Transport.Exceptions {
    [Serializable]
    public class ConnectionException : TransportException {
        public ConnectionException() { }

        public ConnectionException(string address)
            : base(String.Format("connection to {0} failed", address)) { }
    }
}