using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.Core.Messages
{
    internal class MqttConnectMessage : MqttMessage
    {
        /// <summary>
        /// The version of the MQTT protocol we are using
        /// </summary>
        private const byte VERSION = 3;

        private ushort _keepAlive;
        private byte[] _clientID;

        private byte _connectFlags;
        private bool _containsWill;
        private string _willTopic;
        private byte[] _willPayload;

        /// <summary>
        /// Constant description of the protocol
        /// </summary>
        private byte[] protocolDesc = new byte[]
            {
                0,
                6,
                (byte)'M',
                (byte)'Q',
                (byte)'I',
                (byte)'s',
                (byte)'d',
                (byte)'p',
                VERSION
            };

        private void SetConnectVariableHeaderCommon(string clientID, ushort keepAlive)
        {
          _keepAlive = keepAlive;
          _clientID = enc.GetBytes(clientID);
          base.variableHeaderLength = (
            protocolDesc.Length + //Length of the protocol description
            3 +                   //Connect Flags + Keep alive
            _clientID.Length +    // Length of the client ID string
            2                     // The length of the length of the clientID
          );
        }

        public MqttConnectMessage(string clientID, ushort keepAlive, bool cleanStart)
          : base(MessageType.CONNECT)
        {
          SetConnectVariableHeaderCommon(clientID, keepAlive);
          _containsWill = false;
          _connectFlags = (byte)(cleanStart ? 0x02:0) ;
        }

        // TODO: Add a constructor containing WillTopic and WillMessage
        public MqttConnectMessage(
          string clientID, ushort keepAlive,
          string willTopic, byte[] willPayload,
          QoS willQos, bool willRetained, bool cleanStart
        ) : base(MessageType.CONNECT)
        {
          SetConnectVariableHeaderCommon(clientID, keepAlive);

          _containsWill = true;
          _willTopic = willTopic;
          _willPayload = willPayload;

          _connectFlags = (byte) (
            0x04                      | // LWT enabled
            (willRetained ? 0x20 : 0) | // LWT is retained?
            (cleanStart ? 0x02 : 0)   | // Clean Start
            ((byte)willQos) << 3        // LWT QoS
          );

          base.variableHeaderLength += (
            _willTopic.Length +
            _willPayload.Length +
            4
          );
        }

        protected override void SendPayload(System.IO.Stream str)
        {
            str.Write(protocolDesc, 0, protocolDesc.Length);

            // TODO: Implement Clean Session Flag
            str.WriteByte(_connectFlags);

            // Write the keep alive value
            WriteToStream(str, _keepAlive);
            
            // Write the payload
            WriteToStream(str, (ushort)_clientID.Length);
            str.Write(_clientID, 0, _clientID.Length);

            if (_containsWill)
            {
              // Write the will topic
              WriteToStream(str, _willTopic);

              // Write the will payload
              WriteToStream(str, (ushort)_willPayload.Length);
              str.Write(_willPayload, 0, _willPayload.Length);
            }

        }

        protected override void ConstructFromStream(System.IO.Stream str)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
