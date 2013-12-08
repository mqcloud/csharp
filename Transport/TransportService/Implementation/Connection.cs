using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using MQCloud.Transport.Extensions;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Protocol;
using ZeroMQ;

namespace MQCloud.Transport.Service.Implementation {
    internal class Connection : IDisposable {
        private Configuration _config;

        private readonly NetworkManager _networkManager;
        private readonly NodesController _nodesController;
        private readonly SocketInformer _gatewayInformer = new SocketInformer();
        private readonly List<string> _addresses = new List<string>();
        private readonly Poller _pooler;

        private void ReadConfiguation(string configureationFile) {
            try {
                using ( var fs = File.Open( configureationFile, FileMode.Open, FileAccess.Read ) ) {
                    var serializer = new XmlSerializer( typeof( Configuration ) );
                    _config = (Configuration)serializer.Deserialize( fs );
                }
            } catch ( FileNotFoundException ) {
                _config = new Configuration();

                var writer = new XmlSerializer( typeof( Configuration ) );
                var file = new StreamWriter( configureationFile );
                writer.Serialize( file, _config );
                file.Close();
            }
        }

        public Connection(NetworkManager networkManager, string configureationFile = "MQCloud.Transport.Service.config") {
            _networkManager = networkManager;
            ReadConfiguation( configureationFile );

            _nodesController = new NodesController( _networkManager, _config.PingSettings.Interval, _config.PingSettings.Policy );

            _gatewayInformer.Socket = _networkManager.CreateSocket( SocketType.ROUTER );
            _gatewayInformer.Address = _networkManager.OpenSocket( _gatewayInformer.Socket );

            _gatewayInformer.Socket.ReceiveReady +=
                (sender, args) => {
                    var message = args.GetProtocolOperationRequest<OperationGetBaseChannelsFacadeRequest>();
                    var response = message.Message.GetResponse();

                    if ( !message.Topic.Equals( Informer.GatewayBaseDataTopic ) ) {
                        response.State = OperationStatusCode.OperationStatusCodeError;
                        response.Error = "Wrong Topic!";
                        args.Socket.SendProtocolOperationResponse(
                            response, message.Topic, message.SenderId, message.CallbackId
                        );
                    }

                    var request = message.Message;
                    var guid = string.Format( "{0}@{1}", Guid.NewGuid().ToString(), request.ApplicationName );

                    _addresses.Add( guid );
                    response.EventTopics = new List<string> { TransportInformer.GatewayPingChannalName };
                    response.EventsAddress = _nodesController.EventsInformer.Address;
                    response.OperationsAddress = _nodesController.OperationsInformer.Address;
                    response.UserId = guid;
                    args.Socket.SendProtocolOperationResponse( response, message.Topic, message.SenderId, message.CallbackId );
                };

            _pooler = new Poller( new List<ZmqSocket> { _gatewayInformer.Socket } );
            ThreadPool.QueueUserWorkItem( state => _pooler.PoolerLoop( new TimeSpan( 0, 0, 1 ) ) );
        }

        public void Dispose() {
            _pooler.Dispose();

            _networkManager.FreeSocket( _gatewayInformer.Socket );
            _gatewayInformer.Socket.Dispose();
        }
    }
}