using System;
using System.Text;
using MqttLib.Core;
using MqttLib;

namespace MqttLib
{
    public class MqttClientFactory
    {
        public static IMqtt CreateClient(string connString, string clientId, string username, string password, IPersistence persistence)
        {
            return new Mqtt(connString, clientId, username, password, persistence);
        }

        public static IMqtt CreateClient(string connString, string clientId)
        {
            return CreateClient(connString, clientId, null, null, null);
        }

        public static IMqttShared CreateSharedClient(string connString, string clientId, string username, string password)
        {
            return new Mqtt(connString, clientId, username, password, null);
        }

        public static IMqttShared CreateSharedClient(string connString, string clientId)
        {
            return new Mqtt(connString, clientId, null, null, null);
        }

        public static IMqtt CreateBufferedClient(string connString, string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
