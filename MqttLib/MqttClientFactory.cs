using System;
using System.Text;
using MqttLib.Core;
using MqttLib;

namespace MqttLib
{
    public class MqttClientFactory
    {
        public static IMqtt CreateClient(string connString, string clientId, string username = null, string password = null, IPersistence persistence = null, bool enableLogging = true)
        {
            return new Mqtt(connString, clientId, username, password, persistence, enableLogging);
        }

        public static IMqttShared CreateSharedClient(string connString, string clientId, string username = null, string password = null, bool enableLogging = true)
        {
            return new Mqtt(connString, clientId, username, password, null, enableLogging);
        }

        public static IMqtt CreateBufferedClient(string connString, string clientId, bool enableLogging = true)
        {
            throw new NotImplementedException();
        }
    }
}
