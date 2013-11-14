using System;

namespace MQCloud.Transport.Implementation
{
    internal class OperationPackageContext {
        public byte[] Data;
        public Action<string> OnError;
        public Action<byte[]> OnResult;
        public TimeSpan Timeout;
    }
}