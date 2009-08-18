using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MqttLib.Core.Messages
{
    internal class MqttUnsubackMessage : MqttAcknowledgeMessage
    {

        public MqttUnsubackMessage(Stream str, byte header) : base(str, header)
        {
          // Nothing to construct
        }

        protected override void ConstructFromStream(System.IO.Stream str)
        {
            _ackID = ReadUshortFromStream(str);
        }
    }
}
