using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MQCloud.Transport.Implementation {
    public delegate void OperationResponseHandler<in TResponse>(TResponse operationResponse);

    public class AsyncOperationsManager<TResponse> { // TODO Move into common
        private int _lastCallbackId;
        private readonly ConcurrentDictionary<int, OperationResponseHandler<TResponse>> _pendingOperations=new ConcurrentDictionary<int, OperationResponseHandler<TResponse>>();

        public int RegisterAsyncOperation(OperationResponseHandler<TResponse> handler) {
            if (handler==null) {
                throw new ArgumentNullException("handler");
            }

            var callbackId=Interlocked.Increment(ref _lastCallbackId);
            _pendingOperations.TryAdd(callbackId, handler);

            return callbackId;
        }

        public bool RemoveCallback(int callbackId){
            OperationResponseHandler<TResponse> handler;
            return _pendingOperations.TryRemove(callbackId, out handler);
        }


        public void ExecuteCallback(int callbackId, TResponse operationResponse) {
            OperationResponseHandler<TResponse> handler;
            _pendingOperations.TryRemove(callbackId, out handler);

            handler(operationResponse);
        }
    }
}