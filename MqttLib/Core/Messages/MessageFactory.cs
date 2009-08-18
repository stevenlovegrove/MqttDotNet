using System;
using System.IO;
using System.Text;

using MqttLib.Logger;

namespace MqttLib.Core.Messages
{
    internal class MessageFactory
    {

        public static MqttMessage CreateMessage(Stream str, byte header)
        {
            // Look at the fixed header to decide what message type we have
            MessageType messageType = (MessageType)((header & 0xf0) >> 4);
            
            //Log.Write( LogLevel.DEBUG, "Message " + messageType + " received");

            switch (messageType)
            {
                case MessageType.CONNACK:
                    return new MqttConnackMessage(str, header);
                case MessageType.DISCONNECT:
                    return null;
                case MessageType.PINGREQ:
                    return new MqttPingReqMessage();
                case MessageType.PUBACK:
                    return new MqttPubackMessage(str, header);
                case MessageType.PUBCOMP:
                    return new MqttPubcompMessage(str, header);
                case MessageType.PUBLISH:
                    return new MqttPublishMessage(str, header);
                case MessageType.PUBREC:
                    return new MqttPubrecMessage(str, header);
                case MessageType.PUBREL:
                    return new MqttPubrelMessage(str, header);
                case MessageType.SUBACK:
                    return new MqttSubackMessage(str, header);
                case MessageType.UNSUBACK:
                    return null;
                case MessageType.PINGRESP:
                    return new MqttPingRespMessage(str, header);
                case MessageType.UNSUBSCRIBE:
                case MessageType.CONNECT:
                
                case MessageType.SUBSCRIBE:
                default:
                    throw new Exception("Unsupported Message Type");

            }
            
        }
    }
}
