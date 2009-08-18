using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.Core
{
    /// <summary>
    /// Thrown when the connection string is invalid
    /// </summary>
    public class MalformedConnectionStringException : Exception
    {
        public MalformedConnectionStringException(string message) : base(message)
        {

        }

        public MalformedConnectionStringException()
        {

        }
    }

    /// <summary>
    /// Thrown when the user tries to create a stream for an unsupported protocol
    /// </summary>
    public class UnsupportedProtocolException : Exception
    {
        public UnsupportedProtocolException(){ }

        public UnsupportedProtocolException(string message) : base(message) { }

    }
}
