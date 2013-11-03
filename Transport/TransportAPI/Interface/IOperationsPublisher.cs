using System;

namespace MQCloud.Transport.Interface {
    public interface IOperationsPublisher {
        void Send(byte[] data, TimeSpan timeout, Action<byte[]> onResult, Action<string> onError);
    }
}