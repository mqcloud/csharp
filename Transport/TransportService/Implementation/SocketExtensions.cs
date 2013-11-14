using System;
using ZeroMQ;

namespace MQCloud.Transport.Service.Implementation {
    internal static class SocketExtensions {
        public static void PoolerLoop(this Poller poller, TimeSpan timspan) {
            while (poller != null) {
                poller.Poll(timspan);
            }
        }
    }
}