using System;
using System.Collections.Concurrent;
using System.Net;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Interface;

namespace MQCloud.Transport {
    public class ConnectionFactory : IConnectionFactory {
        private readonly ConcurrentDictionary<string, NetworkManager> _networkManagers = new ConcurrentDictionary<string, NetworkManager>();

        public IConnection GetConnection(string applicationName, string peerAddress, string hostAddress = "", int hostPort = 5920) {
            NetworkManager networkManager;
            if ( !_networkManagers.TryGetValue( hostAddress, out networkManager ) ) {
                if ( String.IsNullOrEmpty( hostAddress ) && hostAddress != null ) {
                    var destination = IPAddress.Parse( hostAddress );
                    networkManager = new NetworkManager( hostPort, destination );
                } else {
                    networkManager = new NetworkManager( hostPort );
                }
                if ( hostAddress != null ) {
                    _networkManagers.TryAdd( hostAddress, networkManager );
                }
            }
            return new Connection( networkManager, applicationName, peerAddress );
        }
    }
}



