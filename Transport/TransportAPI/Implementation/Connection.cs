using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MQCloud.Transport.Exceptions;
using MQCloud.Transport.Interface;
using MQCloud.Transport.Protocol;
using MQCloud.Transport.Protocol.Utilities;
using ProtoBuf;
using ZeroMQ;

namespace MQCloud.Transport.Implementation {
    internal class Connection : IConnection {
        private NetworkManager Manager { get; set; }
        private string PeerAddress { get; set; }

        private ZmqSocket GatewayConnection { get; set; }
        private ZmqSocket EventsConnection { get; set; }
        private ZmqSocket OperationsConnection { get; set; }

        private void Connect(string peerAdress) {
            GatewayConnection=Manager.CreateSocket(SocketType.DEALER);
            GatewayConnection.Connect(peerAdress);

            GatewayConnection.ReceiveReady+=(sender, args) => OnGatewayResponse(sender, args, stream => {
                var response=Serializer.Deserialize<OperationGetBaseChannelsFacadeResponse>(stream);
                Connect(response);
            });

            var request=new OperationGetBaseChannelsFacadeRequest();

            GatewayConnection.Send(request.ToByteArray());

            var poller=new Poller(new List<ZmqSocket> { GatewayConnection });
            var timeout=new TimeSpan(0, 0, 10); // TODO test;

            poller.Poll(timeout);
            if (EventsConnection==null||OperationsConnection==null) {
                throw new ConnectionException(PeerAddress);
            }


            poller.ClearSockets(); // TODO: check if needed
            Manager.FreeSocket(GatewayConnection); // TODO: check if needed
        }

        private static void OnGatewayResponse(object sender, SocketEventArgs e, Action<MemoryStream> callback) {
            var result=e.Socket.ReceiveMessage();

            var message=result.ToArray()
                .SelectMany(frame => frame.Buffer)
                .ToArray(); //TODO Search for better frame to byte array conversion

            using (var ms=new MemoryStream(message)) {
                callback(ms);
            }
        }

        private void Connect(OperationGetBaseChannelsFacadeResponse context) {
            EventsConnection=Manager.CreateSocket(SocketType.XSUB);
            EventsConnection.Connect(context.EventsAddress);

            OperationsConnection=Manager.CreateSocket(SocketType.DEALER);
            OperationsConnection.Connect(context.OperationsAddress);
        }

        public Connection(NetworkManager manager, string peerAdress) {
            Manager=manager;
            PeerAddress=peerAdress;

            Connect(peerAdress);
        }

        public void SubscribeToOperations(string topic, OperationCallback callback) {
            throw new NotImplementedException();
        }

        public void SubscribeToEvents(string topic, EventCallback callback) {
            throw new NotImplementedException();
        }

        public void UnSubscribeToOperations(string topic) {
            throw new NotImplementedException();
        }

        public void UnSubscribeToEvents(string topic) {
            throw new NotImplementedException();
        }

        public IOperationsPublisher GetOperationsPublisher(string topic) {
            return new OperationsPublisher();
        }

        public IEventsPublisher GetEventsPublisher(string topic) {
            return new EventsPublisher();
        }
    }
}