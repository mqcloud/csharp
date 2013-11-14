using System;
using System.IO;
using MQCloud.Transport.Protocol.Utilities;
using MQCloud.Transport.Service.Test;

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
        private static void TestProtocol() {
            var schema = SchemaPrinter.Print();
            Console.WriteLine(schema);
            File.WriteAllText("TransportProtocol.proto", schema);
        }

        public static void Main() {
            try {
                TestProtocol();
                //  ZmqEventsTest.TestZmqEvents();
                ZmqOperationsTest.TestZmqOperations();
            } catch (Exception e) {
                Console.Write(e);
                Console.ReadKey();
            }
        }
    }
}