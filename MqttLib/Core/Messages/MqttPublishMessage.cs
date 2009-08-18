using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.Core.Messages
{
    internal class MqttPublishMessage : MqttMessage
    {
      private string _topic;
      private byte[] _payload;

      public MqttPublishMessage( ushort id, string topic, byte[] payload, QoS qos, bool retained )
        : base(MessageType.PUBLISH)
      {
        _topic = topic;
        _payload = payload;
        _messageID = id;

        base.msgQos = qos;
        base.isRetained = retained;

        base.variableHeaderLength =
          2 + GetUTF8StringLength(topic) +    // Topic + length
          (qos == QoS.BestEfforts ? 0 : 2) +  // Message ID for QoS > 0
          payload.Length;                     // Message Payload
      }

      public MqttPublishMessage(System.IO.Stream str, byte header) : base(str, header) { }

      protected override void SendPayload(System.IO.Stream str)
      {
        WriteToStream(str, _topic);
        if (msgQos != QoS.BestEfforts) WriteToStream(str, _messageID);
        str.Write(_payload, 0, _payload.Length);
      }

      protected override void ConstructFromStream(System.IO.Stream str)
      {
        int payloadLen = base.variableHeaderLength;

        _topic = ReadStringFromStream(str);
        payloadLen -= (GetUTF8StringLength(_topic) + 2);

        if (msgQos != QoS.BestEfforts) {
          _messageID = ReadUshortFromStream(str);
          payloadLen -= 2;
        }

        _payload = new byte[payloadLen];

        ReadCompleteBuffer(str, _payload);
      }

        public string Topic
        {
            get
            {
                return _topic;
            }
        }

        public MqttPayload Payload
        {
            get
            {
                return new MqttPayload(_payload, 0);
            }
        }

    }
}
