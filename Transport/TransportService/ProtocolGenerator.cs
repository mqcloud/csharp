using System;
using System.IO;
using MQCloud.Transport.Protocol.Utilities;

namespace MQCloud.Transport.Service
{
    public class ProtocolGenerator {
        public static void Save(string fileName = "TransportProtocol.proto") {
            var schema = SchemaPrinter.Print();
            Console.WriteLine( schema );
            File.WriteAllText( fileName, schema );
        }
    }
}