using System.Linq;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Protocol;
using ZeroMQ;

namespace MQCloud.Transport.Extensions {
    internal static class OperationRequestExtensions {
        public static void SendProtocolOperationRequest(this ZmqSocket socket, Message message, string topic, string senderId, int callbackId) {
            socket.SendUserOperationRequest(message.ToByteArray(), topic, senderId, callbackId);
        }

        public static void SendUserOperationRequest(this ZmqSocket socket, byte[] message, string topic, string senderId, int callbackId) {
            var packet=new ZmqMessage();
            packet.Append(senderId.ToByteArray());
            packet.Append(topic.ToByteArray());
            packet.Append(callbackId.ToByteArray());
            packet.Append(message);
            socket.SendMessage(packet);
        }

        public static string GetOperationRequestSenderId(this ZmqMessage e) {
            return e.First.Buffer.ToUtf8();
        }

        public static string GetOperationRequestTopic(this ZmqMessage e) {
            return e.Skip(1).First().Buffer.ToUtf8();
        }

        public static int GetOperationRequestCallackId(this ZmqMessage e) {
            return e.Skip(2).First().Buffer.ToInt32();
        }

        public static ThematicUserOperationRequest GetUserOperationRequest(this SocketEventArgs e) {
            var message=e.GetMessage();

            return new ThematicUserOperationRequest {
                Topic=message.GetOperationRequestTopic(),
                CallbackId=message.GetOperationRequestCallackId(),
                SenderId=message.GetOperationRequestSenderId(),
                Message=message.GetBytes(3)
            };
        }

        public static ThematicOperationRequest<T> GetProtocolOperationRequest<T>(this SocketEventArgs e) where T : Message {
            var message=e.GetMessage();

            return new ThematicOperationRequest<T> {
                Topic=message.GetOperationRequestTopic(),
                CallbackId=message.GetOperationRequestCallackId(),
                SenderId=message.GetOperationRequestSenderId(),
                Message=message.GetBytes(3).ToMessage<T>()
            };
        }
    }
}