using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security;

#if WITH_BLUETOOTH
using InTheHand.Net.Sockets;
using InTheHand.Net;
#endif // WITH_BLUETOOTH

namespace MqttLib.Core
{
    public class StreamFactory
    {
        // TODO: Let Brenton add comments!
        public const string PARAM_HOST = "HOST";
        public const string PARAM_PORT = "PORT";

        public const string DIV_SCHEME = "://";
        public const string DIV_PORT = ":";
        public const string DIV_PARAM = ";";
        public const string DIV_VALUE = "=";

        // Broker Bluetooth Connector Service
        public static Guid BROKER_SERVICE = new Guid("{CA1C1BD6-7D53-4065-B0EE-AEB49AAAC0C3}");

        // Over-ride default constructor so that this factory can't be instantiated
        private StreamFactory()
        {
        }

        /// <summary>
        /// Create input and output streams based on the given connection string
        /// </summary>
        /// <param name="connString">Connection String</param>
        /// <returns>A Stream object</returns>
        public static Stream CreateStream(string connStr)
        {
            /* Move to external function documentation
             * 
             * Connection String format:
             * scheme://<Resource ID>:<port>;<param1=value1>;<params2=value2>...
             * Note: <> denotes optional
             * 
             */

            // Locate Scheme segment. If it doesn't exist, throw an exception
            int locScheme = connStr.IndexOf(DIV_SCHEME);
            if( locScheme < 0 ) throw new MalformedConnectionStringException("Could not find " + DIV_SCHEME + " in the connection string");

            string scheme = connStr.Substring( 0, locScheme ).ToUpper();
            connStr = connStr.Substring( locScheme + DIV_SCHEME.Length );

            Hashtable parameters = null;

            // Extract any parameters that exist
            int paramLoc = connStr.IndexOf(DIV_PARAM);
            if( paramLoc >= 0 ) {
                string paramStr = connStr.Substring( paramLoc + DIV_PARAM.Length );
                parameters = GetParameters( paramStr );
                connStr = connStr.Substring(0, paramLoc);
            }

            if (parameters == null)
            {
                parameters = new Hashtable();
            }

            // Extract port if it exists
            int portLoc = connStr.LastIndexOf( DIV_PORT);
            if( portLoc >= 0 ) {
                String portStr = connStr.Substring(portLoc + DIV_PORT.Length);
                parameters.Add( PARAM_PORT, portStr );
                connStr = connStr.Substring(0, portLoc);
            }

            // We're left with the host
            parameters.Add( PARAM_HOST, connStr );

            switch( scheme ) {
                case "TCP":
                    return CreateTcpStreams( parameters );
#if WITH_BLUETOOTH
                case "BT":
                    return CreateBluetoothStreams(parameters);
#endif // WITH_BLUETOOTH
                  default:
                    throw new UnsupportedProtocolException(scheme + " is not supported at this time.");
            }
        }

        private static NetworkStream CreateTcpStreams( Hashtable parms )
        {
            string host = (string)parms[PARAM_HOST];
            int port = Convert.ToInt32(parms[PARAM_PORT]);

            TcpClient tcpclnt = new TcpClient();
            try
            {
                tcpclnt.Connect(host, port);
                return tcpclnt.GetStream();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

#if WITH_BLUETOOTH
        private static NetworkStream CreateBluetoothStreams(Hashtable parms)
        {
          string hostaddr = (string)parms[PARAM_HOST];
          MqttLib.Logger.Log.Write(MqttLib.Logger.LogLevel.INFO, "StreamFactory: Creating Bluetooth Stream to " + hostaddr );

          //BluetoothClient bc = new BluetoothClient();
          FixedBluetoothClient bc = new FixedBluetoothClient();
          BluetoothAddress bta = BluetoothAddress.Parse( hostaddr );
          MqttLib.Logger.Log.Write(MqttLib.Logger.LogLevel.INFO, "StreamFactory: Parsed BT address as " + bta.ToString());
          MqttLib.Logger.Log.Write(MqttLib.Logger.LogLevel.INFO, "StreamFactory: BT connecting " );
          bc.Connect(new BluetoothEndPoint(bta, BROKER_SERVICE));
          MqttLib.Logger.Log.Write(MqttLib.Logger.LogLevel.INFO, "StreamFactory: BT connected ");
          NetworkStream ns = bc.GetStream();
          bc.Close();
          return ns;
        }
#endif //WITH_BLUETOOTH

        private static Hashtable GetParameters(string paramList)
        {
            Hashtable parms = new Hashtable();

            while( paramList != null && paramList.Length > 0 )
            {
                int pos = paramList.IndexOf(DIV_PARAM);
                string pair;
                if (pos > 0) {
                  pair = paramList.Substring(0, pos);
                  paramList = paramList.Substring(pos + DIV_PARAM.Length);
                } else {
                  pair = paramList;
                  paramList = null;
                }
                pos = pair.IndexOf(DIV_VALUE);
                if (pos > 0) {
                  string param = pair.Substring(0, pos).ToUpper();
                  string value = pair.Substring(pos + DIV_VALUE.Length);
                  parms.Add(param, value);
                }
            }
            return parms;
        }

    }

#if WITH_BLUETOOTH
  class FixedBluetoothClient : BluetoothClient
  {
    public FixedBluetoothClient()
      :base() { }

    protected override void Dispose(bool disposing)
    {
      // Steven Lovegrove: Compensation for the bad logic in original src
      base.Dispose(false);
    }
  }
#endif // WITH_BLUETOOTH

}
