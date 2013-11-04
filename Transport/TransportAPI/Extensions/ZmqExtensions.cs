using System.Linq;
using ZeroMQ;

namespace MQCloud.Transport.Extensions
{
    internal static class ZmqExtensions {
        public static ZmqMessage GetMessage(this SocketEventArgs e) {
            return e.Socket.ReceiveMessage();
        }

        public static byte[] GetBytes(this ZmqMessage e, int skeepFrames=1) {
            return e.Skip(skeepFrames)
                    .SelectMany(frame => frame.Buffer)
                    .ToArray(); //TODO: Search for better frame to byte array conversion
        }
    }
}