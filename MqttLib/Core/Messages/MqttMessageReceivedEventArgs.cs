using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.Core.Messages
{
    internal class MqttMessageReceivedEventArgs : EventArgs
    {
        private MqttMessage _message = null;

        public MqttMessageReceivedEventArgs(MqttMessage message)
        {
            _message = message;
        }

        public MqttMessage Message
        {
            get
            {
                return _message;
            }
        }
    }
}
