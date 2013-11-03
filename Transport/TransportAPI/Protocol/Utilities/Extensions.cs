using System.IO;
using ProtoBuf;

namespace MQCloud.Transport.Protocol.Utilities {
    internal static class Extensions {
        public static byte[] ToByteArray(this Message message) {
            using (var ms=new MemoryStream()) {
                Serializer.Serialize(ms, message);
                return ms.GetBuffer();
            }
        }
    }
}