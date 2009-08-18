using System;

namespace MqttLib.Core
{
    public class Subscription
    {
        private string _topic;
        private QoS _qos;

        public Subscription(string topic, QoS qos)
        {
            _topic = topic;
            _qos = qos;
        }

        public string Topic
        {
            get
            {
                return _topic;
            }
        }

        public QoS QualityOfService
        {
            get
            {
                return _qos;
            }
        }
    }
}
