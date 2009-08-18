using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.Logger
{
    public enum LogLevel : uint
    {
        DEV = 0,
        DEBUG = 1,
        INFO = 2,
        ERROR = 3,
        CRITICAL = 4
    }
    public interface ILog
    {
        void Write(string message);

        void Write(LogLevel level, string message);

        LogLevel LoggingLevel {get; set;}
    }
}
