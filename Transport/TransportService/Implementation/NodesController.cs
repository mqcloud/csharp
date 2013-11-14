using System;
using System.Collections.Generic;
using System.Threading;
using MQCloud.Transport.Extensions;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Protocol;
using ZeroMQ;

namespace MQCloud.Transport.Service.Implementation {
    internal class NodesController : IDisposable {
        private NetworkManager NetworkManager { get; set; }

        private readonly List<NodeInformer> _nodes = new List<NodeInformer>();
        private readonly Timer _pingTimer;
        private readonly Poller _pooler;

        public SocketInformer EventsInformer { get; set; }
        public SocketInformer OperationsInformer { get; set; }

        public NodesController(NetworkManager networkManager, int pingInterval, int pingPolicy) {
            NetworkManager = networkManager;

            EventsInformer.Socket = NetworkManager.CreateSocket(SocketType.XPUB);
            EventsInformer.Address = NetworkManager.OpenSocket(EventsInformer.Socket);

            OperationsInformer.Socket = NetworkManager.CreateSocket(SocketType.ROUTER);
            OperationsInformer.Address = NetworkManager.OpenSocket(OperationsInformer.Socket);

            _pingTimer = new Timer(
                state => {
                    lock (_nodes) {
                        _nodes.ForEach(informer => ++informer.Counter);
                        _nodes.RemoveAll(informer => informer.Counter > pingPolicy);
                    }

                    EventsInformer.Socket
                                  .SendProtocolEvent(new EventPingRequest(), TransportInformer.GatewayPingChannalName);
                },
                null,
                (long)pingInterval,
                pingInterval
                );

            _pooler = new Poller(new List<ZmqSocket> { EventsInformer.Socket, OperationsInformer.Socket });
            ThreadPool.QueueUserWorkItem(state => _pooler.PoolerLoop(new TimeSpan(0, 0, 1)));
        }

        public void Dispose() {
            _pingTimer.Dispose();
            _pooler.Dispose();

            NetworkManager.FreeSocket(EventsInformer.Socket);
            NetworkManager.FreeSocket(OperationsInformer.Socket);

            EventsInformer.Socket.Dispose();
            OperationsInformer.Socket.Dispose();
        }
    }
}