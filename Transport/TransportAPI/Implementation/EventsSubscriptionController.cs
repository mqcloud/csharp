using System;
using System.Collections.Generic;
using System.Threading;
using MQCloud.Transport.Extensions;
using MQCloud.Transport.Interface;
using MQCloud.Transport.Protocol;
using ZeroMQ;

namespace MQCloud.Transport.Implementation
{
    internal class EventsSubscriptionController {
        private EventCallback Callback { get; set; }
        private NetworkManager NetworkManager { get; set; }
        private string Topic { get; set; }

        private Poller _poller;
        private ZmqSocket _socket;

        private void PoolerLoop() {
            var timspan=new TimeSpan(0, 0, 1);
            while (_poller!=null) {
                _poller.Poll(timspan);
            }
        }

        private void OnRequest(object sender, SocketEventArgs e) {
            var message=e.GetUserEvent();
            if (message.Topic.Equals(Topic)) {
                Callback(message.Message);
            }
        }

        public EventsSubscriptionController(EventCallback callback, NetworkManager networkManager, string topic) {
            Callback=callback;
            NetworkManager=networkManager;
            Topic=topic;
        }

        public void SubscriobeToEventsCallback(OperationGetEventsPublisherResponse operationResponse) {
            _socket=NetworkManager.CreateSocket(SocketType.XSUB);
            operationResponse.Addresses.ForEach(_socket.Connect);
            _socket.Subscribe(Topic.ToByteArray());
            _socket.ReceiveReady+=OnRequest;

            _poller=new Poller(new List<ZmqSocket> { _socket });

            ThreadPool.QueueUserWorkItem(state => PoolerLoop());
        }

        public void UpdateSubscriptions(EventPeers context) // TODO: check if threadsafe
        {
            context.Added.ForEach(_socket.Connect);
            context.Removed.ForEach(_socket.Disconnect);
        }
    }
}