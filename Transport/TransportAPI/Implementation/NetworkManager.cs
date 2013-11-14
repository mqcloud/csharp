using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using MQCloud.Transport.Exceptions;
using ZeroMQ;
using SocketType = ZeroMQ.SocketType;

namespace MQCloud.Transport.Implementation {
    internal class NetworkManager {
        private readonly ZmqContext _context=ZmqContext.Create();

        private readonly List<int> _occupiedPorts=new List<int>();
        private readonly List<ZmqSocket> _openedSockets=new List<ZmqSocket>();

        public NetworkManager(int port = 5920, IPAddress address=null) {
            if (address==null) {
                address=Resolve();
            }
            Address=address;
            _occupiedPorts.Add(--port);
        }

        private IPAddress Address { get; set; }

        private static IPAddress Resolve() {
            foreach (var address in
                Dns.GetHostEntry(Dns.GetHostName()).AddressList
                   .Where(address => address.AddressFamily==AddressFamily.InterNetwork)) {
                return address;
            }
            throw new IpNotFound();
        }

        private int GetFreePort() {
            _occupiedPorts.Add(_occupiedPorts.Last()+1);
            return _occupiedPorts.Last();
        }

        public ZmqSocket CreateSocket(SocketType type) {
            var result=_context.CreateSocket(type);
            lock (_openedSockets) {
                _openedSockets.Add(result);
            }
            return result;
        }

        public string OpenSocket(ZmqSocket socket) {
            var result="";
            var connected=false;
            while (!connected) {
                try {
                    result=string.Format("{0}:{1}", Address, GetFreePort());
                    socket.Bind(result);
                    connected=true;
                } catch (ZmqSocketException) {
                    //TODO: log exception!
                }
            }

            return result;
        }

        public void FreeSocket(ZmqSocket socket) {
            socket.Close();
            lock (_openedSockets) {
                _openedSockets.Remove(socket);
            }
        }
    }
}