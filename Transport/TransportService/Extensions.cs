using System.Text;

namespace TransportService {
    public static class Extensions {
        public static byte[] ToByteArray(this string data) {
            return Encoding.UTF8.GetBytes(data);
        }

        public static string ToUtf8(this byte[] data) {
            return Encoding.UTF8.GetString(data);
        }
    }
}