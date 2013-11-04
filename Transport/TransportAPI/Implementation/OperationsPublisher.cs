using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using MQCloud.Transport.Interface;
using MQCloud.Transport.Protocol;
using MQCloud.Transport.Extensions;
using ZeroMQ;

namespace MQCloud.Transport.Implementation {
    internal class OperationsPublisher : IOperationsPublisher {
        private readonly string _topic;
        private readonly string _senderId;
        private readonly NetworkManager _networkManager;
        private readonly List<OperationPackageContext> _packets=new List<OperationPackageContext>();
        private readonly List<PendingOperationContext> _pendingOperations=new List<PendingOperationContext>();
        private readonly AsyncOperationsManager<byte[]> _asyncOperationsManager=new AsyncOperationsManager<byte[]>();

        private Timer RemoveOutOfDateOperaqtionsTimer { get; set; }

        private Poller _poller;
        private ZmqSocket _socket;

        private void OnResponse(object sender, SocketEventArgs e) {
            lock (_packets) {
                lock (_pendingOperations) {
                    _packets.ForEach(
                        packageContext => {
                            var callbackId=_asyncOperationsManager.RegisterAsyncOperation(
                                operationResponse => packageContext.OnResult(operationResponse));

                            e.Socket.SendUserOperationRequest(
                                packageContext.Data,
                                _topic,
                                _senderId,
                                callbackId
                            );

                            var outDate=DateTime.UtcNow+packageContext.Timeout;

                            _pendingOperations.Add(new PendingOperationContext {
                                OperationCallbackId=callbackId,
                                OutDate=outDate
                            });
                        });
                }

                _packets.Clear();
            }
        }

        private void OnRequest(object sender, SocketEventArgs e) {
            var operation=e.GetUserOperationRequest();
            _asyncOperationsManager.ExecuteCallback(operation.CallbackId, operation.Message);
        }

        private void PoolerLoop() {
            var timspan=new TimeSpan(0, 0, 1);
            while (_poller!=null) {
                _poller.Poll(timspan);
            }
        }

        private void CleanUpLoop() {
            lock (_pendingOperations) {
                var now=DateTime.UtcNow;
                var zero=new TimeSpan(0, 0, 0);
                var outOfDateOperations=_pendingOperations.Where(
                    context => (now-context.OutDate>zero)
                        &&_asyncOperationsManager.RemoveCallback(context.OperationCallbackId));

                outOfDateOperations.ForEach(context => {
                    context.OnError("Out Of Date");
                    _pendingOperations.Remove(context);
                });
            }
        }

        public void SubscriobeToOperationsCallback(OperationGetOperationsPublisherResponse context) {
            if (context.State==OperationStatusCode.OperationStatusCodeError) {
                // TODO: handle exceptional case
                return;
            }
            _socket=_networkManager.CreateSocket(SocketType.DEALER);
            context.Addresses.ForEach(_socket.Connect); //TODO: make connections updatable

            _socket.SendReady+=OnResponse;
            _socket.ReceiveReady+=OnRequest;

            _poller=new Poller(new List<ZmqSocket> { _socket });

            ThreadPool.QueueUserWorkItem(state => PoolerLoop());
        }

        public void UpdateSubscriptions(EventPeers context) { // TODO: check if threadsafe
            context.Added.ForEach(_socket.Connect);
            context.Removed.ForEach(_socket.Disconnect);
        }

        public OperationsPublisher(string topic, string senderId, NetworkManager networkManager) {
            _topic=topic;
            _networkManager=networkManager;
            _senderId=senderId;
            RemoveOutOfDateOperaqtionsTimer=new Timer(state => CleanUpLoop(), null, Informer.MinimalMessageTimeToLive, Informer.MinimalMessageTimeToLive);
        }

        public void Send(byte[] data, TimeSpan timeout, Action<byte[]> onResult, Action<string> onError) {
            lock (_packets) {
                _packets.Add(new OperationPackageContext {
                    Data=data,
                    OnError=onError,
                    OnResult=onResult,
                    Timeout=timeout
                });
            }
        }
    }
}