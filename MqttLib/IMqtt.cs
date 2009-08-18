using System;
using System.Collections.Generic;
using System.Text;
using MqttLib.Core;

namespace MqttLib
{
  /// <summary>
  /// An MQTT control interface mirroring that of the IBM IA92 Java API.
  /// Users of an IMqtt object must handle their own subscription processing and
  /// division.
  /// </summary>
  public interface IMqtt : IMqttPublisher, IMqttSubscriber, IMqttConnectDisconnect
  {
    /// <summary>
    /// Get and Set interval (in milliseconds) to wait before resending unacknowledged messages
    /// </summary>
    long ResendInterval { get; set; }
  }
}
