using System;
using MQCloud.Transport.Interface;

namespace MQCloud.Transport.Implementation {
    internal class OperationsPublisher : IOperationsPublisher {
        public void Send(byte[] data, TimeSpan timeout, Action<byte[]> onResult, Action<string> onError) {
            throw new NotImplementedException();
        }
    }
}