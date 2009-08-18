using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MqttLib.Core.Messages;
using MqttLib.Logger;

namespace MqttLib.Core
{
    internal class StreamManager
    {
        private Stream _channel = null;
        private byte[] headerBuffer = new byte[1];
        private string _connString;
        private bool _connected = false;
        private bool _closing = false;

        AsyncCallback callback;
        QoSManager qosManager;
        IAsyncResult readOp = null;

        public bool IsConnected
        {
            get
            {
                return _connected;
            }
        }

        public StreamManager(string connString, QoSManager qosMan)
        {
            _connString = connString;
            qosManager = qosMan;
            callback = new AsyncCallback(listen);
        }

        public void Connect()
        {
            _channel = StreamFactory.CreateStream(_connString);
            readOp = _channel.BeginRead(headerBuffer, 0, 1, callback, null);
            // Give the qosManager a handle to the streams
            qosManager.SetStreamManager(this);
            _connected = true;
        }

        public void WaitForResponse()
        {
          if (readOp != null)
          {
            readOp.AsyncWaitHandle.WaitOne();
          }
        }

        public void Disconnect()
        {
            _connected = false;
            _closing = true;

            // Wait for any pending read operations to complete
            if (readOp != null)
            {
              // KLUDGE: for some streams, commented code deadlocks.
              // TODO: Fix at source
              //_channel.EndRead(readOp);
              readOp = null;
            }

            // Trigger QoS Manager to terminate
            qosManager.Running = false;

            _channel.Close();
        }

        public void SendMessage(MqttMessage mess)
        {
            if (_connected)
            {
                qosManager.ProcessSentMessage(mess);
                mess.Serialise(_channel);
            }
            else
            {
                throw new NotConnectedException("Not connected to any stream,  call Connect() first");
            }
        }


        private void listen(IAsyncResult asyncResult)
        {
          if (_closing) return;

          // Invalidate readOp Object to signify processing in this method
          readOp = null;

          try
          {
            _channel.EndRead(asyncResult);
            MqttMessage msg = MessageFactory.CreateMessage(_channel, headerBuffer[0]);
            qosManager.ProcessReceivedMessage(msg);
            readOp = _channel.BeginRead(headerBuffer, 0, 1, callback, null);
          }
          catch (Exception e)
          {
            Log.Write(LogLevel.ERROR, e.ToString());

            if (_connected)
            {
              qosManager.Running = false;
              _connected = false;
              //Process as null message to signify a disconnect
              qosManager.ProcessReceivedMessage(null);
            }
          }
                       
        }

        

    }
}
