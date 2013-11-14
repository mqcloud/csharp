using System;

namespace MQCloud.Transport.Implementation {
    internal class PendingOperationContext {
        public Action<string> OnError;
        public int OperationCallbackId;
        public DateTime OutDate;
    }
}