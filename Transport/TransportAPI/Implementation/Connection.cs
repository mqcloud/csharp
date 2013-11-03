using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using MQCloud.Transport.Exceptions;
using MQCloud.Transport.Extensions;
using MQCloud.Transport.Interface;
using MQCloud.Transport.Protocol;
using MQCloud.Transport.Protocol.Utilities;
using ProtoBuf;
using ZeroMQ;
using System.Collections.Concurrent;

namespace MQCloud.Transport.Implementation {
    internal class Connection : IConnection {
        private const string GatewayBaseDataTopic="B|"+Informer.Version;
        private const string GatewayEventsTopic="E|"+Informer.Version;
        private const string GatewayOperationsTopic="O|"+Informer.Version;

        private NetworkManager NetworkManager { get; set; }
        private string PeerAddress { get; set; }

        private ZmqSocket GatewayConnection { get; set; }
        private ZmqSocket GatewayEventsListner { get; set; }
        private ZmqSocket GatewayOperationsExchange { get; set; }

        private ZmqSocket EventsPublisher { get; set; }
        private ZmqSocket OperationsExchange { get; set; }


        private readonly Poller _gatewayPoller=new Poller();

        private readonly ConcurrentDictionary<string, OperationsPublisher> _operationsPublishers=new ConcurrentDictionary<string, OperationsPublisher>();
        private readonly AsyncOperationsManager<OperationResponse> _asyncOperationsManager=new AsyncOperationsManager<OperationResponse>();

        private void Connect(string peerAdress) {
            GatewayConnection=NetworkManager.CreateSocket(SocketType.DEALER);
            GatewayConnection.Connect(peerAdress);

            GatewayConnection.ReceiveReady+=(sender, e) => {
                var response=e.GetMessage<OperationGetBaseChannelsFacadeResponse>();
                Connect(response.Message);
            };

            var request=new OperationGetBaseChannelsFacadeRequest();

            GatewayConnection.Send(request, GatewayBaseDataTopic);
            _gatewayPoller.AddSocket(GatewayConnection);
            var timeout=new TimeSpan(0, 0, 10); // TODO: test;

            _gatewayPoller.Poll(timeout);

            if (GatewayEventsListner==null||GatewayOperationsExchange==null) {
                throw new ConnectionException(PeerAddress);
            }
            _gatewayPoller.ClearSockets();
            NetworkManager.FreeSocket(GatewayConnection);

            GatewayOperationsExchange.ReceiveReady+=(sender, args) => {
                var response=args.GetMessage<OperationResponse>().Message;
                _asyncOperationsManager.ExecuteCallback(response.CallbackId, response);
            };

            GatewayEventsListner.ReceiveReady+=(sender, args) => {
                var response=args.GetMessage<Event>().Message;
                switch ((EventTypeCode)response.TypeAttributes[1]) { //TODO: add events handling
                    case EventTypeCode.Peers:
                    case EventTypeCode.Ping:

                    default:
                    break;
                }
            };
            _gatewayPoller.AddSockets(new List<ZmqSocket> { GatewayOperationsExchange, GatewayEventsListner });
            ThreadPool.QueueUserWorkItem(GatewayPooler);
        }

        private void GatewayPooler(object state) {
            var timeout=new TimeSpan(0, 0, 1); // TODO: test;
            while (_gatewayPoller!=null) //TODO: handle loop exit
            {
                _gatewayPoller.Poll(timeout);
            }
        }

        private void Connect(OperationGetBaseChannelsFacadeResponse context) {
            GatewayEventsListner=NetworkManager.CreateSocket(SocketType.XSUB);
            context.EventTopics.ForEach(s => GatewayEventsListner.Subscribe(s.ToByteArray()));
            GatewayEventsListner.Connect(context.EventsAddress);

            GatewayOperationsExchange=NetworkManager.CreateSocket(SocketType.DEALER);
            GatewayOperationsExchange.Connect(context.OperationsAddress);

            EventsPublisher=NetworkManager.CreateSocket(SocketType.XPUB);
            NetworkManager.OpenSocket(EventsPublisher);

            OperationsExchange=NetworkManager.CreateSocket(SocketType.ROUTER);
            NetworkManager.OpenSocket(OperationsExchange);
        }

        public Connection(NetworkManager manager, string peerAdress) {
            NetworkManager=manager;
            PeerAddress=peerAdress;

            Connect(peerAdress);
        }

        public bool SubscribeToOperations(string topic, OperationCallback callback) {
            throw new NotImplementedException();
        }

        public bool SubscribeToEvents(string topic, EventCallback callback) {
            throw new NotImplementedException();
        }

        public void UnSubscribeToOperations(string topic) {
            throw new NotImplementedException();
        }

        public void UnSubscribeToEvents(string topic) {
            throw new NotImplementedException();
        }

        public IOperationsPublisher GetOperationsPublisher(string topic) {
            OperationsPublisher result;
            if (!_operationsPublishers.TryAdd(topic, null)) {
                _operationsPublishers.TryGetValue(topic, out result);
            } else {
                result=new OperationsPublisher(topic, NetworkManager);
                var request=new OperationGetOperationsPublisherRequest {
                    Topic=topic,
                    CallbackId=_asyncOperationsManager.RegisterAsyncOperation(
                        response => result.SubscriobeToOperationsCallback(
                            (OperationGetOperationsPublisherResponse)response))
                };
                GatewayOperationsExchange.Send(request, GatewayOperationsTopic);
                _operationsPublishers.TryUpdate(topic, result, null);
            }
            return result;
        }

        public IEventsPublisher GetEventsPublisher(string topic) {
            return new EventsPublisher();
        }
    }
}