namespace MQCloud.Transport.Service.Implementation {
    internal static class TransportInformer {
        public const string GatewayPingChannalSuffix = "|PING";
        public const string GatewayPingChannalName = Informer.GatewayEventsTopic + GatewayPingChannalSuffix;
    }
}