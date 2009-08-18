using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.Logger
{
    public class Log
    {
      private static ILog _log = null;

      public static ILog CreateLog(string name)
      {
        if (_log == null) { _log = new FileLog(name); }
        return _log;
      }

      public static ILog CreateLog(string path, string name)
      {
        if (_log == null) { _log = new FileLog(path, name); }
        return _log;
      }

      public static void Write(string message)
      {
          if (_log == null) { CreateLog("Unknown"); }
          _log.Write(message);
      }

      public static void Write(LogLevel level, string message)
      {
          if (_log == null) { CreateLog("Unknown"); }
          _log.Write(level, message);
      }
    }
}
