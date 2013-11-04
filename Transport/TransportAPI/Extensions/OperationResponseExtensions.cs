using System.Linq;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Protocol;
using ZeroMQ;

namespace MQCloud.Transport.Extensions {
    internal static class OperationResponseExtensions {

        public static void SendProtocolOperationResponse(this ZmqSocket socket, Message message, string topic, string senderId, int callbackId) {
            socket.SendUserOperationResponse(message.ToByteArray(), topic, senderId, callbackId);
        }

        public static void SendUserOperationResponse(this ZmqSocket socket, byte[] message, string topic, string senderId, int callbackId) {
            socket.SendUserOperationRequest(message, topic, senderId, callbackId);
        }

        public static string GetOperationResponseTopic(this ZmqMessage e) {
            return e.First.Buffer.ToUtf8();
        }

        public static int GetOperationResponseCallackId(this ZmqMessage e) {
            return e.Skip(1).First().Buffer.ToInt32();
        }

        public static ThematicUserOperationResponse GetUserOperationResponse(this SocketEventArgs e) {
            var message=e.GetMessage();

            return new ThematicUserOperationResponse {
                Topic=message.GetOperationResponseTopic(),
                CallbackId=message.GetOperationResponseCallackId(),
                Message=message.GetBytes(2)
            };
        }

        public static ThematicOperationResponse<T> GetProtocolOperationResponse<T>(this SocketEventArgs e) where T : Message {
            var message=e.GetMessage();

            return new ThematicOperationResponse<T> {
                Topic=message.GetOperationResponseTopic(),
                CallbackId=message.GetOperationResponseCallackId(),
                Message=message.GetBytes(2).ToMessage<T>()
            };
        }
    }
}