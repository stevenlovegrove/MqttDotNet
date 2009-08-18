using System;
using System.Text;
using System.IO;

namespace MqttLib.Core.Messages
{
    /// <summary>
    /// The different types of messages defined in the MQTT protocol
    /// </summary>
    public enum MessageType : byte
    {
    	 CONNECT 		= 1,
	     CONNACK 		= 2,
	     PUBLISH 		= 3,
	     PUBACK 		= 4,
	     PUBREC 		= 5,
	     PUBREL 		= 6,
	     PUBCOMP 		= 7,
	     SUBSCRIBE 	    = 8,
	     SUBACK 		= 9,
	     UNSUBSCRIBE 	= 10,
	     UNSUBACK 		= 11,
	     PINGREQ 		= 12,
	     PINGRESP 		= 13,
	     DISCONNECT 	= 14
    }

    internal abstract class MqttMessage : MqttLib.Core.Messages.IPersitentMessage
    {
        // UTF8 encoding for all strings
        protected UTF8Encoding enc = new UTF8Encoding();
       
        protected MessageType msgType;
        private bool isDuplicate = false;

        
        protected bool isRetained = false;
        protected ushort _messageID = 0;
        private long _timestamp;

        public long Timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                _timestamp = value;
            }
        }

        public bool Retained
        {
            get { return isRetained; }
        }

        public bool Duplicate
        {
            get { return isDuplicate; }
            set { isDuplicate = value; }
        }

        public ushort MessageID
        {
            get
            {
                return _messageID;
            }
        }

        protected QoS msgQos       = QoS.BestEfforts;
        protected int variableHeaderLength = 0;

        public MqttMessage(MessageType msgType, int variableHeaderLength )
        {
            this.variableHeaderLength = variableHeaderLength;
            this.msgType = msgType;
            this._timestamp = DateTime.Now.Ticks;
            
        }

        public MqttMessage(MessageType msgType)
        {
            this.msgType = msgType;
            this._timestamp = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Creates an MqttMessage from a data stream
        /// </summary>
        /// <param name="str">Input stream</param>
        /// <param name="header">The first byte of the fixed header of the message</param>
        public MqttMessage(Stream str, byte header)
        {
            this._timestamp = DateTime.Now.Ticks;
            ConstructHeader(header);
            variableHeaderLength = DecodeVariableHeaderLength(str);
            ConstructFromStream(str);
        }


        private void ConstructHeader(byte header)
        {
            msgType = (MessageType)((header & 0xf0) >> 4);
            isDuplicate = (header & 0x08) != 0;
            msgQos = (QoS)(header & 0x06);
            isRetained = (header & 0x01) != 0;
        }

        /// <summary>
        /// Decodes the length of the variable header and payload from the given stream
        /// </summary>
        /// <param name="str">Input Stream</param>
        /// <returns>Length of the variable header and the payload</returns>
        private int DecodeVariableHeaderLength(Stream str)
        {
            int multiplier = 1 ;
            int value = 0 ;
            int digit = 0;

            do 
            {
                digit = str.ReadByte();
                if (digit == -1)
                {
                    return 0;
                }
                value += (digit & 127) * multiplier; 
                multiplier *= 128;
            }
            while ((digit & 128) != 0);
          
            return value;
        }

        /// <summary>
        /// Encodes the length of the variable header to the format specified in the MQTT protocol
        /// and writes it to the given stream
        /// </summary>
        /// <param name="str">Output Stream</param>
        /// <param name="length">Length of variable header and payload</param>
        private void EncodeVariableHeaderLength(Stream str, int length)
        {
            byte digit = 0;
            do
            {
                digit = (byte)(length % 128);
                length /= 128;
                if (length > 0)
                {
                    digit |= 0x80;
                }
                str.WriteByte(digit);
            }
            while ( length > 0);

        }

        public void Serialise(Stream str)
        {
            // Write the fixed header to the stream
            byte header = (byte)((byte)msgType << 4);
            if (isDuplicate)
            {
                header |= 8;
            }
            header |= (byte)((byte)msgQos << 1);
            if (isRetained)
            {
                header |= 1;
            }

            str.WriteByte(header);
            // Add the second byte of the fixed header (The variable header length)
            EncodeVariableHeaderLength(str, variableHeaderLength);
            // Write the payload to the stream
            SendPayload(str);
        }

        /// <summary>
        /// Concrete Message classes should write their variable header and payload to the given stream
        /// </summary>
        /// <param name="str">Output Stream</param>
        protected virtual void SendPayload(Stream str) 
        {
            throw new Exception("Protocol does not support sending this message type");
        }

        /// <summary>
        /// Concrete Message classes should extract their variable header and payload from the given stream
        /// </summary>
        /// <param name="str">Input Stream</param>
        protected virtual void ConstructFromStream(Stream str)
        {
            throw new Exception("Protocol does not support receiving this type of message");
        }


        protected static void WriteToStream(Stream str, ushort val)
        {
            str.WriteByte((byte)(val >> 8));
            str.WriteByte((byte)(val & 0xFF));
        }

        protected static void WriteToStream(Stream str, string val)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] bs = enc.GetBytes(val);
            WriteToStream(str, (ushort)bs.Length);
            str.Write(bs, 0, bs.Length);
        }

        protected static ushort ReadUshortFromStream(Stream str)
        {
          // Read two bytes and interpret as ushort in Network Order
          byte[] data = new byte[2];
          ReadCompleteBuffer( str, data );
          return (ushort)((data[0] << 8) + data[1]);
        }

        protected static string ReadStringFromStream(Stream str)
        {
          ushort len = ReadUshortFromStream(str);
          byte[] data = new byte[len];
          ReadCompleteBuffer(str, data);
          UTF8Encoding enc = new UTF8Encoding();
          return enc.GetString(data, 0, data.Length);
        }

        protected static byte[] ReadCompleteBuffer(Stream str, byte[] buffer)
        {
          int read = 0;
          while (read < buffer.Length)
          {
            int res = str.Read(buffer, read, buffer.Length - read);
            if (res == -1)
            {
                throw new Exception("End of stream reached whilst filling buffer");
            }
            read += res;
          }
          return buffer;
        }

        protected static int GetUTF8StringLength(string s)
        {
            UTF8Encoding enc = new UTF8Encoding();
            return enc.GetByteCount(s);
        }

        public MessageType MsgType
        {
            get
            {
                return msgType;
            }
        }

        public QoS QualityOfService
        {
            get
            {
                return msgQos;
            }
        }

    }
}
