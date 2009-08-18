using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib
{
    public class MqttBrokerUnavailableException : MqttException
    {

        public MqttBrokerUnavailableException() : base() { }

        public MqttBrokerUnavailableException(string message) : base(message) { }

        public MqttBrokerUnavailableException(string message, Exception innerException) : base(message, innerException) { }

    }
}
