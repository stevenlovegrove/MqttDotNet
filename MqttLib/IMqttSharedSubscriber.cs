using System;
using System.Collections.Generic;
using System.Text;
using MqttLib.Core;

namespace MqttLib
{
  public interface IMqttSharedSubscriber
  {
    /// <summary>
    /// Register subscription to subscriber. Mqtt client will automatically subscribe to
    /// requested topic if previous subscriber hasn't already registered for it.
    /// </summary>
    /// <param name="subscription">Subscription request</param>
    /// <param name="subscriber">Delegate for method to be called if a message is received for this subscription</param>
    void Subscribe(Subscription subscription, PublishArrivedDelegate subscriber);

    /// <summary>
    /// Unregister subscriber for the specified topic. If this is the last subscriber for
    /// the given topic, the Mqtt client will perform an Mqtt unsubscribe.
    /// </summary>
    /// <param name="topic">Topic to unsubscribe from</param>
    /// <param name="subscriber">The subscriber to unregister</param>
    void Unsubscribe(string topic, PublishArrivedDelegate subscriber);
  }
}
