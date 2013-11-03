using System;
using System.Collections.Concurrent;
using System.Net;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Interface;

namespace MQCloud.Transport {
    public class ConnectionFactory : IConnectionFactory {
        private readonly ConcurrentDictionary<string, NetworkManager> _networkManagers=new ConcurrentDictionary<string, NetworkManager>();

        public IConnection GetConnection(string peerAddress, string hostAddress="") {
            NetworkManager networkManager;
            if (_networkManagers.TryGetValue(hostAddress, out networkManager)) {

            } else {
                if (String.IsNullOrEmpty(hostAddress)&&hostAddress!=null) {
                    var destination=IPAddress.Parse(hostAddress);
                    networkManager=new NetworkManager(destination);
                } else {
                    networkManager=new NetworkManager();

                }
                if (hostAddress!=null)
                    _networkManagers.TryAdd(hostAddress, networkManager);
            }
            return new Connection(networkManager, peerAddress);
        }

    }
}



