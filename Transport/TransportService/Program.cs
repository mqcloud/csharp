#region

using System;
using System.IO;
using MQCloud.Transport.Protocol.Utilities;

#endregion

namespace TransportService {
    internal class Program {
        private static void TestProtocol() {
            var schema=SchemaPrinter.Print();
            Console.WriteLine(schema);
            File.WriteAllText("TransportProtocol.proto", schema);
        }

        private static void Main() {
            try {
                TestProtocol();
                //  ZmqEventsTest.TestZmqEvents();
                //ZmqOperationsTest.TestZmqOperations();
            } catch (Exception e) {
                Console.Write(e);
                Console.ReadKey();
            }
        }
    }
}
