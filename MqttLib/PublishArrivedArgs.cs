using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib
{
  public class PublishArrivedArgs : EventArgs
  {

    #region Member variables

    private string _topic;
    private MqttPayload _payload;
    private bool _retained;
    private QoS _qos;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructs a PublishArrivedArgs object
    /// </summary>
    /// <param name="topic">Source of message</param>
    /// <param name="payload">Message body</param>
    /// <param name="retained">Whether or not the message is retained</param>
    public PublishArrivedArgs(string topic, MqttPayload payload, bool retained, QoS qos)
    {
      _topic = topic;
      _payload = payload;
      _retained = retained;
      _qos = qos;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Whether or not this message is a retained copy sent by the broker
    /// </summary>
    public bool Retained
    {
      get
      {
        return _retained;
      }
    }

    /// <summary>
    /// Source of the message
    /// </summary>
    public string Topic
    {
      get
      {
        return _topic;
      }
    }

    /// <summary>
    /// Message body
    /// </summary>
    public MqttPayload Payload
    {
      get
      {
        return _payload;
      }
    }

      /// <summary>
      /// Quality of service
      /// </summary>
      public QoS QualityOfService
      {
          get
          {
              return _qos;
          }
      }

    #endregion

  }
}
