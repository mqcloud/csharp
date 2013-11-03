namespace MQCloud.Transport.Interface {
    public interface IConnectionFactory {
        IConnection GetConnection(string peerAddress, string hostAddress="");
    }
}