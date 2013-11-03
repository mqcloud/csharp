using System;
using System.Collections.Generic;
using ZeroMQ;

namespace MQCloud.Transport.Service {
    internal class ZmqEventsTest {
        private static int _sentCounter;

        public static void TestZmqEvents() {
            _sentCounter=0;

            var context=ZmqContext.Create();
            var publisher=context.CreateSocket(SocketType.XPUB);
            publisher.Bind("tcp://*:4780");

            var subscriberA=context.CreateSocket(SocketType.XSUB);
            subscriberA.Connect("tcp://*:4780");
            subscriberA.Subscribe("A".ToByteArray());


            var subscriberB=context.CreateSocket(SocketType.XSUB);
            subscriberB.Connect("tcp://*:4780");
            subscriberB.Subscribe("B".ToByteArray());

            subscriberA.ReceiveReady+=OnMessageToA;
            subscriberB.ReceiveReady+=OnMessageToB;
            publisher.SendReady+=OnSender;

            var poller=new Poller(new List<ZmqSocket> { publisher, subscriberA, subscriberB });

            var timeout=new TimeSpan(0, 0, 1);

            while (true) {
                poller.Poll(timeout);
            }
        }

        private static void OnMessageToA(object sender, SocketEventArgs e) {
            var result=e.Socket.ReceiveMessage();
            if (result.FrameCount<2||!result[0].Buffer.ToUtf8().Equals("A")) {
                Console.WriteLine("error on A");
            }
        }

        private static void OnMessageToB(object sender, SocketEventArgs e) {
            var result=e.Socket.ReceiveMessage();
            if (result.FrameCount<2||!result[0].Buffer.ToUtf8().Equals("B")) {
                Console.WriteLine("error on B");
            }
        }

        private static void OnSender(object sender, SocketEventArgs e) {
            var m2A=new ZmqMessage();
            m2A.Append("A".ToByteArray());
            m2A.Append("data".ToByteArray());
            e.Socket.SendMessage(m2A);

            var m2B=new ZmqMessage();
            m2B.Append("B".ToByteArray());
            m2B.Append("data 2".ToByteArray());
            e.Socket.SendMessage(m2B);

            if ((++_sentCounter%10000)==0) {
                Console.WriteLine("sent {0}", _sentCounter);
            }

        }
    }
}