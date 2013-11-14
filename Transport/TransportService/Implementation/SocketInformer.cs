using System.Collections.Generic;
using ZeroMQ;

namespace MQCloud.Transport.Service.Implementation {
    internal class SocketInformer {
        public string Address;
        public SocketType Type;
        public readonly List<string> Topics = new List<string>();
        public ZmqSocket Socket;
    }
}