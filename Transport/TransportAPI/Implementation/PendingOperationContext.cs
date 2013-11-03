using System;

namespace MQCloud.Transport.Implementation
{
    internal class PendingOperationContext
    {
        public DateTime OutDate;
        public int OperationCallbackId;
        public Action<string> OnError;
    }
}