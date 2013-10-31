using System;
using System.Collections.Concurrent;
using System.Net;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Interface;

namespace MQCloud.Transport {
    public class ConnectionFactory : IConnectionFactory {
        private readonly ConcurrentDictionary<string, NetworkManager> _networkManagers=new ConcurrentDictionary<string, NetworkManager>();

        public IConnection GetConnection(string address) {
            NetworkManager networkManager;
            if (_networkManagers.TryGetValue(address, out networkManager)) {

            } else
            {
                if (String.IsNullOrEmpty(address) && address  != null) {
                    var destination=IPAddress.Parse(address);
                    networkManager=new NetworkManager(destination);
                } else {
                    networkManager=new NetworkManager();

                }
                if (address != null) _networkManagers.TryAdd(address, networkManager);
            }
            return new Connection(networkManager);
        }
    }
}



