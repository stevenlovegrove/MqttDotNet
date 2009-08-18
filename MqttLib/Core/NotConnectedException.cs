using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.Core
{
    public class NotConnectedException : Exception
    {
        public NotConnectedException() : base() { }

        public NotConnectedException(string message) : base(message) { }
    }
}
