using System;
using System.Collections.Generic;
using System.Text;
using MqttLib.Core;

namespace MqttLib.Core.Messages
{
    internal class MqttSubscribeMessage : MqttMessage
    {
        private Subscription[] _subscriptions = null;
        /// <summary>
        /// Subscribe to multiple topics
        /// </summary>
        /// <param name="subscriptions">Array of Subscription objects</param>
        public MqttSubscribeMessage(ushort messageID, Subscription[] subscriptions) : base(MessageType.SUBSCRIBE)
        {
            _messageID = messageID;
            _subscriptions = subscriptions;
            base.msgQos = QoS.AtLeastOnce;
            // Work out the length of the payload
            int payloadLength = 0;
            foreach (Subscription s in _subscriptions)
            {
              payloadLength += (2 + GetUTF8StringLength(s.Topic) + 1);
            }

            this.variableHeaderLength = (
                                            2 + // Length of message ID
                                            payloadLength // Length of the payload
                                        );
        }

        protected override void SendPayload(System.IO.Stream str)
        {
            WriteToStream(str, _messageID);
            // Write the subscription payload
            foreach (Subscription s in _subscriptions)
            {
                WriteToStream(str, s.Topic);
                str.WriteByte((byte)s.QualityOfService);
            }
        }

        protected override void ConstructFromStream(System.IO.Stream str)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
