namespace MQCloud.Transport.Exceptions {
    public class IpNotFound : TransportException {
        public IpNotFound()
            : base("Could not detect IP") {
        }
    }
}