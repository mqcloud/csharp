namespace MQCloud.Transport.Service.Implementation {
    internal class NodeInformer {
        public volatile int PingFramesCallbackDelayCounter;
        public string Name { get; set; }
    }
}