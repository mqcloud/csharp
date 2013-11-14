namespace MQCloud.Transport
{
    internal static class Informer
    {
        public const string Version = "V1";
        public const int MinimalMessageTimeToLive=10000;
        public const string GatewayBaseDataTopic="B|"+Version+"|";
        public const string GatewayEventsTopic="E|"+Version+"|";
        public const string GatewayOperationsTopic="O|"+Version+"|";
    }
}