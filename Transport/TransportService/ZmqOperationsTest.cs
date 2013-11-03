using System;
using System.Collections.Generic;
using System.Linq;
using ZeroMQ;

namespace MQCloud.Transport.Service {
    internal class ZmqOperationsTest {
        public static void TestZmqOperations() {
            var context=ZmqContext.Create();

            var serverA=context.CreateSocket(SocketType.ROUTER);
            serverA.Bind("tcp://*:4782");

            var serverB=context.CreateSocket(SocketType.ROUTER);
            serverB.Bind("tcp://*:4783");

            var client=context.CreateSocket(SocketType.DEALER);
            client.Identity="client".ToByteArray();
            client.Connect("tcp://localhost:4782");
            client.Connect("tcp://localhost:4783");

            client.ReceiveReady+=OnServerResponse;
            serverA.ReceiveReady+=(s, a) => OnServerRequest(s, a, "SA");
            serverB.ReceiveReady+=(s, a) => OnServerRequest(s, a, "SB");

            var poller=new Poller(new List<ZmqSocket> { client, serverA, serverB });

            var timeout=new TimeSpan(0, 0, 5);

            while (true) {
                poller.Poll(timeout);
                client.Send("Hello".ToByteArray());
            }
        }

        private static void OnServerResponse(object sender, SocketEventArgs e) {
            var result=e.Socket.ReceiveMessage();

            var message=result.Aggregate("", (current, f) => current+f.Buffer.ToUtf8()+"\n");
            Console.Write("client receiverd: {0}", message);
        }

        private static void OnServerRequest(object sender, SocketEventArgs e, string serverName) {
            var server=e.Socket;
            var result=server.ReceiveMessage();

            if (result.FrameCount>1) {
                var senderId=result[0].Buffer.ToUtf8();
                var message=new byte[0];
                for (var i=1; i<result.FrameCount; ++i) {
                    message=message.Concat(result[i].Buffer).ToArray();
                }

                Console.WriteLine("server {0} received: {1}, from {2}", serverName, message.ToUtf8(), senderId);

                server.SendMore(senderId.ToByteArray());
                server.Send("world".ToByteArray());
            }
        }
    }
}