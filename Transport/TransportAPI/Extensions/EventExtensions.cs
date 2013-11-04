using MQCloud.Transport.Implementation;
using MQCloud.Transport.Protocol;
using ZeroMQ;

namespace MQCloud.Transport.Extensions {
    internal static class EventExtensions {
        public static void SendProtocolEvent(this ZmqSocket socket, Message message, string topic) {
            socket.SendUserEvent(message.ToByteArray(), topic);
        }

        public static void SendUserEvent(this ZmqSocket socket, byte[] message, string topic) {
            var packet=new ZmqMessage();
            packet.Append(topic.ToByteArray());
            packet.Append(message);
            socket.SendMessage(packet);
        }

        public static string GetEventTopic(this ZmqMessage e) {
            return e.First.Buffer.ToUtf8();
        }

        public static ThematicUserEvent GetUserEvent(this SocketEventArgs e) {
            var message=e.GetMessage();

            return new ThematicUserEvent {
                Topic=message.GetEventTopic(),
                Message=message.GetBytes()
            };
        }

        public static ThematicEvent<T> GetProtocolEvent<T>(this SocketEventArgs e) where T : Message{
            var message=e.GetMessage();

            return new ThematicEvent<T> {
                Topic=message.GetEventTopic(),
                Message=message.GetBytes().ToMessage<T>()
            };
        }

    }
}