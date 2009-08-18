using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MqttLib.Core.Messages
{
    internal class MqttPingRespMessage : MqttMessage
    {

        public MqttPingRespMessage() : base(MessageType.PINGRESP, 0)
        {
            // Nothing to construct
        }

        public MqttPingRespMessage(Stream str, byte header) : base(str, header)
        {
            // Nothing to construct
        }

        protected override void SendPayload(System.IO.Stream str)
        {
            // Nothing to send
        }

        protected override void ConstructFromStream(System.IO.Stream str)
        {
            // Nothing to construct
        }
    }
}
