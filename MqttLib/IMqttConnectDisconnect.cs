using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib
{
  public interface IMqttConnectDisconnect
  {
    /// <summary>
    /// Connect to the MQTT message broker
    /// </summary>
    void Connect();

    /// <summary>
    /// Connect to the MQTT message broker
    /// </summary>
    /// <param name="cleanStart">If true, all previous subscriptions and pending messages for client are lost</param>
    void Connect(bool cleanStart);

    /// <summary>
    /// Connect to the MQTT message broker specifying last will & testament (LWT)
    /// </summary>
    /// <param name="willTopic">Destination of LWT</param>
    /// <param name="willQoS">QoS of LWT</param>
    /// <param name="willMsg">Message body of LWT</param>
    /// <param name="willRetain">Whether LWT is retained</param>
    void Connect(string willTopic, QoS willQoS, MqttPayload willMsg, bool willRetain);

    /// <summary>
    /// Connect to the MQTT message broker specifying last will & testament (LWT)
    /// </summary>
    /// <param name="willTopic">Destination of LWT</param>
    /// <param name="willQoS">QoS of LWT</param>
    /// <param name="willMsg">Message body of LWT</param>
    /// <param name="willRetain">Whether LWT is retained</param>
    /// <param name="cleanStart">If true, all previous subscriptions and pending messages for client are lost</param>
    void Connect(string willTopic, QoS willQoS, MqttPayload willMsg, bool willRetain, bool cleanStart);

    /// <summary>
    /// Disconnect from the MQTT message broker
    /// </summary>
    void Disconnect();

    /// <summary>
    /// Fired when the connection to the broker is lost
    /// </summary>
    event ConnectionDelegate ConnectionLost;

    /// <summary>
    /// Fired when a connection is made with a broker
    /// </summary>
    event ConnectionDelegate Connected;

    /// <summary>
    /// Returns true if the client is connected to a broker, false otherwise
    /// </summary>
    bool IsConnected { get;}

    /// <summary>
    /// Interval (in seconds) in which Client is expected to Ping Broker to keep session alive.
    /// If this interval expires without communication, the broker will assume the client
    /// is disconnected, close the channel, and broker any last will and testament contract.
    /// </summary>
    ushort KeepAliveInterval { get; set;}
  }
}
