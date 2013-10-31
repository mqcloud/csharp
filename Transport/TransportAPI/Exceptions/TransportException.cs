using System;

namespace MQCloud.Transport.Exceptions
{
    public class TransportException : Exception {
        public TransportException() { }
        public TransportException(string message) : base(message) { }

    }
}