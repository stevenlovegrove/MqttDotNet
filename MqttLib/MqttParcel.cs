using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib
{
  public class MqttParcel
  {
    protected string topic;
    public string Topic
    {
      get { return topic; }
    }

    protected MqttPayload payload;
    public MqttPayload Payload
    {
      get { return payload; }
    }

    protected QoS qos;
    public QoS Qos
    {
      get { return qos; }
    }

    protected bool retained;
    public bool Retained
    {
      get { return retained; }
    }

    public MqttParcel(string topic, MqttPayload payload, QoS qos, bool retained )
    {
      this.topic = topic;
      this.payload = payload;
      this.qos = qos;
      this.retained = retained;
    }

    public MqttParcel(string topic, string payload, QoS qos, bool retained)
    {
      this.topic = topic;
      this.payload = new MqttPayload(payload);
      this.qos = qos;
      this.retained = retained;
    }

    public MqttParcel(string topic, byte[] payload, QoS qos, bool retained)
    {
      this.topic = topic;
      this.payload = new MqttPayload(payload,0);
      this.qos = qos;
      this.retained = retained;
    }

  }
}
