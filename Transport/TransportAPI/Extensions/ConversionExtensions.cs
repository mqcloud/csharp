using System;
using System.IO;
using System.Net;
using System.Text;
using MQCloud.Transport.Protocol;
using ProtoBuf;

namespace MQCloud.Transport.Extensions
{
    internal static class ConversionExtensions
    {
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

        public static T ToMessage<T>(this byte[] message) where T : Message {
            using (var ms=new MemoryStream(message)) {
                return Serializer.Deserialize<T>(ms);
            }
        }
    }
}