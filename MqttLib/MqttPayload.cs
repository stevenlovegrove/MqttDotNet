using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib
{
  /// <summary>
  /// MQTT Message Body
  /// </summary>
  public class MqttPayload
  {
    #region Member variables

    private byte[] _payload;
    private int _offset;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructs an MQTT message body containing data payload starting from offset
    /// to end of payload
    /// </summary>
    /// <param name="payload">Body of message</param>
    /// <param name="offset">Message start offset</param>
    public MqttPayload(byte[] payload, int offset)
    {
      _offset = offset;
      _payload = payload;
    }

    /// <summary>
    /// Constructs am MQTT message body containing the given string
    /// </summary>
    /// <param name="payload">Body of message</param>
    public MqttPayload(string payload)
    {
        UTF8Encoding enc = new UTF8Encoding();
        _offset = 0;
        _payload = enc.GetBytes(payload) ?? new byte[0]; // might be null for string.empty
    }

    #endregion

    #region Functions

    /// <summary>
    /// Buffer containing message payload
    /// </summary>
	 public byte[] TrimmedBuffer
	 {
		 get
		 {
			 if (_payload.Length - _offset > 0)
			 {
				 byte[] data = new byte[_payload.Length - _offset];
				 _payload.CopyTo(data, _offset);
				 return data;
			 }
			 return new byte[0];
		 }
	 }

    public override string ToString()
    {
      byte[] buffer = TrimmedBuffer;
      if (buffer != null) {
        UTF8Encoding enc = new UTF8Encoding();
        return enc.GetString(buffer, 0, buffer.Length);
      } else {
        return "";
      }
    }

    #endregion

    #region Cast Operators
    /// <summary>
    /// static cast operator to create MqttPayload from a string
    /// </summary>
    /// <param name="str">string payload</param>
    /// <returns>equivelent MqttPayload object</returns>
    public static implicit operator MqttPayload(string str)
    {
      return new MqttPayload(str);
    }

    /// <summary>
    /// static cast operator to create string from MqttPayload
    /// </summary>
    /// <param name="payload">Payload</param>
    /// <returns>equivelent string object</returns>
    public static implicit operator string(MqttPayload payload)
    {
      return payload.ToString();
    }
    #endregion
  }
}
