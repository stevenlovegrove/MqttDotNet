using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MqttLib.Core.Messages
{
    internal class MqttPubrelMessage : MqttMessage
    {
        private ushort _ackID;

        public ushort AckID
        {
            get
            {
                return _ackID;
            }
        }

        public MqttPubrelMessage(ushort ackID) : base(MessageType.PUBREL, 2)
        {
            _ackID = ackID;
        }

        public MqttPubrelMessage(Stream str, byte header) : base(str, header)
        {
            // Nothing to construct
        }

        protected override void SendPayload(System.IO.Stream str)
        {
            WriteToStream(str, _ackID);
        }

        protected override void ConstructFromStream(Stream str)
        {
            _ackID = ReadUshortFromStream(str);
        }
    }
}
