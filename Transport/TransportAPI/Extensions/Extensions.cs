using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Protocol;
using ProtoBuf;
using ZeroMQ;

namespace MQCloud.Transport.Extensions {
    internal static class Extensions {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) { // TODO Move to  common
            foreach (var element in source) {
                action(element);
            }
        }

        public static byte[] ToByteArray(this int value) { // TODO Move to  common
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));
        }

        public static byte[] ToByteArray(this string data) { // TODO Move to  common
            return Encoding.UTF8.GetBytes(data);
        }

        public static string ToUtf8(this byte[] data) { // TODO Move to  common
            return Encoding.UTF8.GetString(data);
        }

        public static int ToInt32(this byte[] data) { // TODO Move to  common
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, 0));
        }

        public static byte[] ToByteArray(this Message message) {
            using (var ms=new MemoryStream()) {
                Serializer.Serialize(ms, message);
                return ms.GetBuffer();
            }
        }

        public static void SendOperation(this ZmqSocket socket, byte[] message, string topic, int callbackId) {
            var packet=new ZmqMessage();
            packet.Append(topic.ToByteArray());
            packet.Append(callbackId.ToByteArray());
            packet.Append(message);
            socket.SendMessage(packet);
        }

        public static void Send(this ZmqSocket socket, Message message, string topic) {
            var packet=new ZmqMessage();
            packet.Append(topic.ToByteArray());
            packet.Append(message.ToByteArray());
            socket.SendMessage(packet);
        }

        public static ZmqMessage GetMessage(this SocketEventArgs e) {
            return e.Socket.ReceiveMessage();
        }

        public static string GetTopic(this ZmqMessage e) {
            return e.First.Buffer.ToUtf8(); //TODO: Search for better frame to byte array conversion
        }

        public static int GetCallackId(this ZmqMessage e) {
            return e.Skip(1).First().Buffer.ToInt32(); //TODO: Search for better frame to byte array conversion
        }

        public static byte[] GetBytes(this ZmqMessage e, int skeepFrames=1) {
            return e.Skip(1)
                .SelectMany(frame => frame.Buffer)
                .ToArray(); //TODO: Search for better frame to byte array conversion
        }

        public static ThematicMessage<T> GetMessage<T>(this SocketEventArgs e) {
            var message=e.GetMessage();

            using (var ms=new MemoryStream(message.GetBytes())) {
                return new ThematicMessage<T> {
                    Topic=message.GetTopic(),
                    Message=Serializer.Deserialize<T>(ms)
                };
            }
        }

        public static ThematicUserOperation GetOperation(this SocketEventArgs e) {
            var message=e.GetMessage();

            return new ThematicUserOperation {
                Topic=message.GetTopic(),
                CallbackId=message.GetCallackId(),
                Message=message.GetBytes(2)
            };
        }
    }
}