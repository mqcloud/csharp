using System;

namespace MQCloud.Transport.Exceptions {
    [Serializable]
    public class IpNotFound : TransportException {
        public IpNotFound()
            : base("Could not detect IP") {
        }
    }
}