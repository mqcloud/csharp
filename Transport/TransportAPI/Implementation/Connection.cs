using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using MQCloud.Transport.Exceptions;
using MQCloud.Transport.Extensions;
using MQCloud.Transport.Interface;
using MQCloud.Transport.Protocol;
using ZeroMQ;

namespace MQCloud.Transport.Implementation {
    internal class Connection : IConnection, IDisposable {
        private readonly AsyncOperationsManager<OperationResponse> _asyncOperationsManager=new AsyncOperationsManager<OperationResponse>();
        private readonly Poller _connectionPoller=new Poller();
        private readonly ConcurrentDictionary<string, EventsSubscriptionController> _eventSubscribers=new ConcurrentDictionary<string, EventsSubscriptionController>();
        private readonly ConcurrentDictionary<string, EventsPublisher> _eventsPublishers=new ConcurrentDictionary<string, EventsPublisher>();
        private readonly Poller _gatewayPoller=new Poller();
        private readonly ConcurrentDictionary<string, OperationCallback> _operationSubscribers=new ConcurrentDictionary<string, OperationCallback>();
        private readonly ConcurrentDictionary<string, OperationsPublisher> _operationsPublishers=new ConcurrentDictionary<string, OperationsPublisher>();

        public Connection(NetworkManager manager, string applicationName, string peerAdress) {
            NetworkManager=manager;
            ApplicationName = applicationName;
            PeerAddress=peerAdress;
            Id=Guid.NewGuid().ToString();
            Connect(peerAdress);
        }

        private NetworkManager NetworkManager { get; set; }
        public string ApplicationName { get; set; }
        private string PeerAddress { get; set; }

        private ZmqSocket GatewayConnection { get; set; }
        private ZmqSocket GatewayEventsListner { get; set; }
        private ZmqSocket GatewayOperationsExchange { get; set; }

        private ZmqSocket EventsPublisher { get; set; }
        private ZmqSocket OperationsExchange { get; set; }

        private string Id { get; set; }

        public bool SubscribeToOperations(string topic, OperationCallback callback) {
            if (!_operationSubscribers.TryAdd(topic, null)) {
                return false;
            }
            var request=new OperationSetOperationsSubscriberRequest {
                Topic=topic,
                Active=true
            };
            var callbackId=_asyncOperationsManager.RegisterAsyncOperation(
                response => { });
            GatewayOperationsExchange.SendProtocolOperationRequest(request, Informer.GatewayOperationsTopic, Id, callbackId);

            return _operationSubscribers.TryUpdate(topic, callback, null);
        }

        public bool SubscribeToEvents(string topic, EventCallback callback) {
            if (!_eventSubscribers.TryAdd(topic, null)) {
                return false;
            }

            var controller=new EventsSubscriptionController(callback, NetworkManager, topic);

            var request=new OperationGetEventsPublisherRequest {
                Topic=topic
            };
            var callbackId=_asyncOperationsManager.RegisterAsyncOperation(
                response => {
                    controller.SubscriobeToEventsCallback(
                        (OperationGetEventsPublisherResponse)response);
                    var infoTopic=Informer.GatewayEventsTopic+topic;
                    GatewayEventsListner.Subscribe(infoTopic.ToByteArray()); // TODO: handle subscription latency issues
                });

            GatewayOperationsExchange.SendProtocolOperationRequest(
                request,
                Informer.GatewayOperationsTopic,
                Id,
                callbackId);

            return _eventSubscribers.TryUpdate(topic, controller, null);
        }

        public bool UnSubscribeFromOperations(string topic)
        {
            var result = false;

            OperationCallback callback;
            if (_operationSubscribers.TryRemove(topic, out callback)) { // TODO : check if socket dispose is needed!
                var request=new OperationSetOperationsSubscriberRequest {
                    Topic=topic,
                    Active=false
                };
                var callbackId=_asyncOperationsManager.RegisterAsyncOperation(
                    response => { });
                GatewayOperationsExchange.SendProtocolOperationRequest(request, Informer.GatewayOperationsTopic, Id, callbackId);

                result = true;
            }

            return result;
        }

        public bool UnSubscribeFromEvents(string topic) {
            EventsSubscriptionController callback;
            return _eventSubscribers.TryRemove(topic, out callback);
        }

        public IOperationsPublisher GetOperationsPublisher(string topic) {
            OperationsPublisher result;
            if (!_operationsPublishers.TryAdd(topic, null)) {
                _operationsPublishers.TryGetValue(topic, out result);
            } else {
                result=new OperationsPublisher(topic, Id, NetworkManager);
                var request=new OperationGetOperationsSubscribersRequest {
                    Topic=topic
                };
                var callbackId=_asyncOperationsManager.RegisterAsyncOperation(
                    response => {
                        result.SubscriobeToOperationsCallback(
                            (OperationGetOperationsSubscribersResponse)response);
                        var infoTopic=Informer.GatewayOperationsTopic+topic;
                        GatewayEventsListner.Subscribe(infoTopic.ToByteArray()); // TODO: handle subscription latency issues
                    });

                GatewayOperationsExchange.SendProtocolOperationRequest(
                    request,
                    Informer.GatewayOperationsTopic,
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
                GatewayOperationsExchange.SendProtocolOperationRequest(request, Informer.GatewayOperationsTopic, Id, callbackId);
                _eventsPublishers.TryUpdate(topic, result, null);
            }
            return result;
        }

        public void Dispose() {
            _gatewayPoller.Dispose();
            _connectionPoller.Dispose();

            GatewayConnection.Close();
            GatewayConnection.Dispose();

            GatewayEventsListner.Close();
            GatewayEventsListner.Dispose();

            GatewayOperationsExchange.Close();
            GatewayOperationsExchange.Dispose();

            EventsPublisher.Close();
            EventsPublisher.Dispose();

            OperationsExchange.Close();
            OperationsExchange.Dispose();
        }

        private void Connect(string peerAdress) {
            GatewayConnection=NetworkManager.CreateSocket(SocketType.DEALER);
            GatewayConnection.Connect(peerAdress);

            GatewayConnection.ReceiveReady+=(sender, e) => {
                                                               var response=e.GetProtocolOperationResponse<OperationGetBaseChannelsFacadeResponse>();
                                                               Connect(response.Message);
            };

            var request = new OperationGetBaseChannelsFacadeRequest { ApplicationName = ApplicationName };

            GatewayConnection.SendProtocolOperationRequest(request, Informer.GatewayBaseDataTopic, Id, 0);
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
            while (_gatewayPoller!=null) { //TODO: handle loop exit
                _gatewayPoller.Poll(timeout);
            }
        }

        private void ConnectionPoller(object state) {
            var timeout=new TimeSpan(0, 0, 1); // TODO: test;
            while (_connectionPoller!=null) { //TODO: handle loop exit
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
                    if (response.Topic.StartsWith(Informer.GatewayOperationsTopic)) {
                        var operationTopic=response.Topic.Replace(Informer.GatewayOperationsTopic, ""); // TODO : replace only first appearence
                        OperationsPublisher publisher;
                        if (_operationsPublishers.TryGetValue(operationTopic, out publisher)) {
                            publisher.UpdateSubscriptions((EventPeers)message);
                        }
                    } else if (response.Topic.StartsWith(Informer.GatewayEventsTopic)) {
                        var eventTopic=response.Topic.Replace(Informer.GatewayEventsTopic, ""); // TODO : replace only first appearence
                        EventsSubscriptionController publisher;
                        if (_eventSubscribers.TryGetValue(eventTopic, out publisher)) {
                            publisher.UpdateSubscriptions((EventPeers)message);
                        }
                    }
                    break;
                }
                case EventTypeCode.Ping: {
                    var request=new OperationPongRequest { Id=Id };

                    GatewayOperationsExchange.SendProtocolOperationRequest(
                        request,
                        Informer.GatewayOperationsTopic,
                        Id,
                        _asyncOperationsManager.RegisterAsyncOperation(operationResponse => { }));
                    break;
                }
            }
        }
    }
}