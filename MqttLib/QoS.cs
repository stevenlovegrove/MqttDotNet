
namespace MqttLib
{
  /// <summary>
  /// Quality of service levels
  /// </summary>
  public enum QoS : byte
  {
    OnceAndOnceOnly = 2,
    AtLeastOnce     = 1,
    BestEfforts     = 0
  }
}