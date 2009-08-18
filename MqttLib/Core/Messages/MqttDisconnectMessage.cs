using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.Core.Messages
{
    internal class MqttDisconnectMessage : MqttMessage
    {
        public MqttDisconnectMessage() : base( MessageType.DISCONNECT, 0 )
        {
          // Nothing to construct
        }

        protected override void SendPayload(System.IO.Stream str)
        {
            // No payload to send
        }

        protected override void ConstructFromStream(System.IO.Stream str)
        {
            throw new Exception("Protocol does not support receiving DISCONNECT message");
        }
    }
}
