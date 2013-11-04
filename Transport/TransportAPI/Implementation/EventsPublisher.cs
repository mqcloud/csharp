using System.Collections.Generic;
using MQCloud.Transport.Interface;
using ZeroMQ;

namespace MQCloud.Transport.Implementation {
    internal class EventsPublisher : IEventsPublisher {
        public ZmqSocket Socket { get; set; }
        public string Topic { get; set; }
        public readonly List<byte[]> _packets=new List<byte[]>();


        public EventsPublisher(ZmqSocket socket, string topic) {
            Socket=socket;
            Topic=topic;
        }

        public void Send(byte[] data) {
            lock (_packets) {
                _packets.Add(data);
            }
        }
    }
}