using System.Collections.Generic;
using ProtoBuf;

namespace MQCloud.Transport.Protocol {
    [ProtoContract]
    [ProtoInclude((int)MessageTypeCode.Operation, typeof(Operation))]
    [ProtoInclude((int)MessageTypeCode.Event, typeof(Event))]
    internal class Message {
        [ProtoMember(1)]
        public List<int> TypeAttributes { get; set; }
        public Message() {
            TypeAttributes=new List<int>();
        }
    }
}

