using System.Collections.Generic;
using MQCloud.Transport.Interface;
using ZeroMQ;

namespace MQCloud.Transport.Implementation {
    internal class EventsPublisher : IEventsPublisher {
        public readonly List<byte[]> Packets=new List<byte[]>();

        public EventsPublisher(ZmqSocket socket, string topic) {
            Socket=socket;
            Topic=topic;
        }

        public ZmqSocket Socket { get; set; }
        public string Topic { get; set; }

        public void Send(byte[] data) {
            lock (Packets) {
                Packets.Add(data);
            }
        }
    }
}