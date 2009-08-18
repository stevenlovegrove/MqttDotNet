using System;
using System.IO;
using System.Text;

namespace MqttLib.Core.Messages
{
    /// <summary>
    /// Possible connection responses
    /// </summary>
    internal enum MqttConnectionResponse : byte
    {
        Accepted                = 0,
        UnacceptableProtocol    = 1,
        IdentifierRejected      = 2,
        BrokerUnavailable       = 3
    }

    internal class MqttConnackMessage : MqttMessage
    {
        private MqttConnectionResponse _response;

        /// <summary>
        /// Contruct a MqttConnackMessage from a given response
        /// </summary>
        /// <param name="response">Connection Repsonse</param>
        public MqttConnackMessage(MqttConnectionResponse response) : base(MessageType.CONNACK, 2)
        {
            _response = response;
        }

        /// <summary>
        /// Construct a MqttConneckMessage from a stream
        /// </summary>
        /// <param name="str"></param>
        /// <param name="header"></param>
        public MqttConnackMessage(Stream str, byte header) : base(str, header) {}

        protected override void SendPayload(System.IO.Stream str)
        {
            // The first byte is reserved for future use
            str.WriteByte(0);
            str.WriteByte((byte)_response);
        }

        protected override void ConstructFromStream(System.IO.Stream str)
        {
            byte[] buffer = new byte[2];
            ReadCompleteBuffer(str, buffer);
            _response = (MqttConnectionResponse)buffer[1];
        }

        /// <summary>
        /// The response from the attemp to connect to a broker
        /// </summary>
        public MqttConnectionResponse Response
        {
            get
            {
                return _response;
            }
        }
    }
}
