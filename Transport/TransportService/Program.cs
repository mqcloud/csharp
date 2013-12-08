using System;
using MQCloud.Transport.Implementation;
using MQCloud.Transport.Service.Test;
using Connection = MQCloud.Transport.Service.Implementation.Connection;

namespace MQCloud.Transport.Service {
    /// <summary>
    /// TransportService aka Name Server aka Transport logic Gateway
    /// ============================================================
    /// 
    /// SHALL BE USED IN SAFE\CLEAN ENVIROMENT!
    /// If you plan to use TransportService node as public end point you shall protect it on your own from DDOS like attacks
    /// Use TCP proxy for protection or any other protection mechanism for load balancing\filtering.
    /// 
    /// </summary>
    internal class Program {


        public static void Main() {
            try {
                ProtocolGenerator.Save();

                var conn = new Connection(new NetworkManager());
            } catch ( Exception e ) {
                Console.Write( e );
                Console.ReadKey();
            }
        }
    }
}