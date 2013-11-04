using System;
using System.Collections.Generic;
using System.Threading;
using MQCloud.Transport.Exceptions;
using MQCloud.Transport.Extensions;
using MQCloud.Transport.Interface;
using MQCloud.Transport.Protocol;
using ZeroMQ;
using System.Collections.Concurrent;

namespace MQCloud.Transport.Implementation {
    internal class Connection : IConnection {
        private const string GatewayBaseDataTopic="B|"+Informer.Version+"|";
        private const string GatewayEventsTopic="E|"+Informer.Version+"|";
        private const string GatewayOperationsTopic="O|"+Informer.Version+"|";

        private NetworkManager NetworkManager { get; set; }
        private string PeerAddress { get; set; }

        private ZmqSocket GatewayConnection { get; set; }
        private ZmqSocket GatewayEventsListner { get; set; }
        private ZmqSocket GatewayOperationsExchange { get; set; }

        private ZmqSocket EventsPublisher { get; set; }
        private ZmqSocket OperationsExchange { get; set; }

        private string Id { get; set; }

        private readonly Poller _gatewayPoller=new Poller();
        private readonly Poller _connectionPoller=new Poller();

        private readonly ConcurrentDictionary<string, OperationsPublisher> _operationsPublishers=new ConcurrentDictionary<string, OperationsPublisher>();
        private readonly ConcurrentDictionary<string, EventsPublisher> _eventsPublishers=new ConcurrentDictionary<string, EventsPublisher>();
        private readonly ConcurrentDictionary<string, OperationCallback> _operationSubscribers=new ConcurrentDictionary<string, OperationCallback>();

        private readonly AsyncOperationsManager<OperationResponse> _asyncOperationsManager=new AsyncOperationsManager<OperationResponse>();

        private void Connect(string peerAdress) {
            GatewayConnection=NetworkManager.CreateSocket(SocketType.DEALER);
            GatewayConnection.Connect(peerAdress);

            GatewayConnection.ReceiveReady+=(sender, e) => {
                var response=e.GetProtocolOperationResponse<OperationGetBaseChannelsFacadeResponse>();
                Connect(response.Message);
            };

            var request=new OperationGetBaseChannelsFacadeRequest();

            GatewayConnection.SendProtocolOperationRequest(request, GatewayBaseDataTopic, Id, 0);
            _gatewayPoller.AddSocket(GatewayConnection);
            var timeout=new TimeSpan(0, 0, 10); // TODO: test;

            _gatewayPoller.Poll(timeout);

            if (GatewayEventsListner==null||GatewayOperationsExchange==null) {
                throw new ConnectionException(PeerAddress);
            }
            _gatewayPoller.ClearSockets();
            NetworkManager.FreeSocket(GatewayConnection);
        }

        private void GatewayPooler(object state) {
            var timeout=new TimeSpan(0, 0, 1); // TODO: test;
            while (_gatewayPoller!=null) //TODO: handle loop exit
            {
                _gatewayPoller.Poll(timeout);
            }
        }

        private void ConnectionPoller(object state) {
            var timeout=new TimeSpan(0, 0, 1); // TODO: test;
            while (_connectionPoller!=null) //TODO: handle loop exit
            {
                _connectionPoller.Poll(timeout);
            }
        }

        private void Connect(OperationGetBaseChannelsFacadeResponse context) {
            Id=context.UserId;

            GatewayEventsListner=NetworkManager.CreateSocket(SocketType.XSUB);
            context.EventTopics.ForEach(s => GatewayEventsListner.Subscribe(s.ToByteArray()));
            GatewayEventsListner.Connect(context.EventsAddress);

            GatewayOperationsExchange=NetworkManager.CreateSocket(SocketType.DEALER);
            GatewayOperationsExchange.Connect(context.OperationsAddress);

            GatewayOperationsExchange.ReceiveReady+=OnGatewayOperationsExchangeOnReceiveReady;

            GatewayEventsListner.ReceiveReady+=OnGatewayEventsListnerOnReceiveReady;

            _gatewayPoller.AddSockets(new List<ZmqSocket> { GatewayOperationsExchange, GatewayEventsListner });
            ThreadPool.QueueUserWorkItem(GatewayPooler);

            EventsPublisher=NetworkManager.CreateSocket(SocketType.XPUB);
            EventsPublisher.SendReady+=(sender, args) => _eventsPublishers.ForEach(pair => {
                lock (pair.Value.Packets) {
                    pair.Value.Packets.ForEach(bytes => EventsPublisher.SendUserEvent(bytes, pair.Key));
                }
            });
            NetworkManager.OpenSocket(EventsPublisher);

            OperationsExchange=NetworkManager.CreateSocket(SocketType.ROUTER);
            OperationsExchange.ReceiveReady+=(sender, args) => {
                var message=args.GetUserOperationRequest();
                OperationCallback callback;
                if (_operationSubscribers.TryGetValue(message.Topic, out callback)) {
                    callback(message.Message,
                             response => OperationsExchange.SendUserOperationResponse(
                                 response,
                                 message.Topic,
                                 message.SenderId,
                                 message.CallbackId));
                }

            };

            NetworkManager.OpenSocket(OperationsExchange);

            _connectionPoller.AddSockets(new List<ZmqSocket> { EventsPublisher, OperationsExchange });
            ThreadPool.QueueUserWorkItem(ConnectionPoller);
        }

        private void OnGatewayOperationsExchangeOnReceiveReady(object sender, SocketEventArgs args) {
            var response=args.GetProtocolOperationResponse<OperationResponse>();
            _asyncOperationsManager.ExecuteCallback(response.CallbackId, response.Message);
        }

        private void OnGatewayEventsListnerOnReceiveReady(object sender, SocketEventArgs args) {
            var response=args.GetProtocolEvent<Event>();
            var message=response.Message;
            switch ((EventTypeCode)message.TypeAttributes[1]) {
                //TODO: add events handling
                case EventTypeCode.Peers: {
                    if (response.Topic.StartsWith(GatewayOperationsTopic)) {
                        var operationTopic=response.Topic.Replace(GatewayOperationsTopic, ""); // TODO : replace only first appearence
                        OperationsPublisher publisher;
                        if (_operationsPublishers.TryGetValue(operationTopic, out publisher)) {
                            publisher.UpdateSubscriptions((EventPeers)message);
                        }
                    } else if (response.Topic.StartsWith(GatewayEventsTopic)) {
                        //TODO: handle
                    }
                    break;
                }
                case EventTypeCode.Ping: {
                    var request=new OperationPongRequest { Id=Id };

                    GatewayOperationsExchange.SendProtocolOperationRequest(
                        request,
                        GatewayOperationsTopic,
                        Id,
                        _asyncOperationsManager.RegisterAsyncOperation(operationResponse => { }));
                    break;
                }
            }
        }

        public Connection(NetworkManager manager, string peerAdress) {
            NetworkManager=manager;
            PeerAddress=peerAdress;
            Id=Guid.NewGuid().ToString();
            Connect(peerAdress);
        }

        public bool SubscribeToOperations(string topic, OperationCallback callback) {
            if (!_operationSubscribers.TryAdd(topic, null)) {
                return false;
            }

            return _operationSubscribers.TryUpdate(topic, callback, null);
        }

        public bool SubscribeToEvents(string topic, EventCallback callback) {
            throw new NotImplementedException();
        }

        public bool UnSubscribeFromOperations(string topic) {
            OperationCallback callback;
            return _operationSubscribers.TryRemove(topic, out callback);
        }

        public bool UnSubscribeFromEvents(string topic) {
            throw new NotImplementedException();
        }

        public IOperationsPublisher GetOperationsPublisher(string topic) {
            OperationsPublisher result;
            if (!_operationsPublishers.TryAdd(topic, null)) {
                _operationsPublishers.TryGetValue(topic, out result);
            } else {
                result=new OperationsPublisher(topic, Id, NetworkManager);
                var request=new OperationGetOperationsPublisherRequest {
                    Topic=topic
                };
                var callbackId=_asyncOperationsManager.RegisterAsyncOperation(
                    response => {
                        result.SubscriobeToOperationsCallback(
                            (OperationGetOperationsPublisherResponse)response);
                        var infoTopic=GatewayOperationsTopic+topic;
                        GatewayEventsListner.Subscribe(infoTopic.ToByteArray()); // TODO: handle subscription latency issues
                    });

                GatewayOperationsExchange.SendProtocolOperationRequest(
                    request,
                    GatewayOperationsTopic,
                    Id,
                    callbackId);
                _operationsPublishers.TryUpdate(topic, result, null);
            }
            return result;
        }

        public IEventsPublisher GetEventsPublisher(string topic) {

            EventsPublisher result;
            if (!_eventsPublishers.TryAdd(topic, null)) {
                _eventsPublishers.TryGetValue(topic, out result);
            } else {
                result=new EventsPublisher(EventsPublisher, topic);
                var request=new OperationSetEventsPublisherRequest {
                    Topic=topic
                };
                var callbackId=_asyncOperationsManager.RegisterAsyncOperation(
                    response => { });
                GatewayOperationsExchange.SendProtocolOperationRequest(request, GatewayOperationsTopic, Id, callbackId);
                _eventsPublishers.TryUpdate(topic, result, null);
            }
            return result;
        }
    }
}