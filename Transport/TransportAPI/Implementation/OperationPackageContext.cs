using System;

namespace MQCloud.Transport.Implementation
{
    internal class OperationPackageContext {
        public byte[] Data;
        public TimeSpan Timeout;
        public Action<byte[]> OnResult;
        public Action<string> OnError;
    }
}