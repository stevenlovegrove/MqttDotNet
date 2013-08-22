using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MqttLib.Core.Messages;
using MqttLib;
using MqttLib.Logger;

namespace MqttLib.Core
{
    internal class QoSManager
    {
        Hashtable _messages = null;
        Hashtable _responses = null;

        IPersistence _store;
        StreamManager _strManager;
        bool _running = false;

        long _resendInterval = 5000;

        /// <summary>
        /// Return true iff the QoS Manager is running
        /// </summary>
        public bool Running
        {
            get { return _running; }
            set { _running = value; }
        }

        /// <summary>
        /// Get and Set interval to wait before resending unacknoledged messages
        /// </summary>
        public long ResendInterval
        {
          get { return _resendInterval; }
          set { _resendInterval = value; }
        }

        public delegate void MessageReceivedDelegate(object sender, MqttMessageReceivedEventArgs e);
        public event MessageReceivedDelegate MessageReceived;

        public QoSManager(IPersistence store)
        {
            _store = store;
            _messages = new Hashtable();
            _responses = new Hashtable();
        }

        public void ProcessReceivedMessage(MqttMessage mess)
        {
          try
          {
            // Check if the message is an acknowledgement
            if (mess is MqttLib.Core.Messages.MqttAcknowledgeMessage)
            {
              MqttAcknowledgeMessage ackMess = (MqttAcknowledgeMessage)mess;
              removeMessage(ackMess.AckID);
            }
            // Check if the message is a PUBREC
            else if (mess is MqttLib.Core.Messages.MqttPubrecMessage)
            {
              MqttPubrecMessage pubrec = (MqttPubrecMessage)mess;
              // Remove the initial publish message
              removeMessage(pubrec.AckID);
              // Send a pubrel message
              _strManager.SendMessage(new MqttPubrelMessage(pubrec.AckID));
            }
            // Check if the message is a PUBCOMP
            else if (mess is MqttLib.Core.Messages.MqttPubcompMessage)
            {
              MqttPubcompMessage pubcomp = (MqttPubcompMessage)mess;
              // Remove the PUBREL message
              removeMessage(pubcomp.AckID);
            }
            else if (mess is MqttLib.Core.Messages.MqttPubrelMessage)
            {
              MqttPubrelMessage pubrel = (MqttPubrelMessage)mess;
              _strManager.SendMessage(new MqttPubcompMessage(pubrel.AckID));
            }

            else if (mess is MqttLib.Core.Messages.MqttPublishMessage)
            {
              if (mess.QualityOfService == QoS.AtLeastOnce)
              {
                // Queue an acknowlegement
                _responses.Add(mess.MessageID, new MqttPubackMessage(mess.MessageID));
              }
              else if (mess.QualityOfService == QoS.OnceAndOnceOnly)
              {
                _responses.Add(mess.MessageID, new MqttPubrelMessage(mess.MessageID));
              }
            }

            // Raise a MessageReceivedEvent
            OnMessageReceived(mess);
          }
          catch (Exception e)
          {
            // we may have just lost the connection
            Log.Write(LogLevel.ERROR, e.ToString());
            // important messages (>QoS0) will be resent by broker
            // we should just ignore the message with no further processing.
          }
        }

        public void ProcessSentMessage(MqttMessage mess)
        {
            // QoS > 0
            if (mess.QualityOfService > QoS.BestEfforts && !mess.Duplicate)
            {
                lock (_messages)
                {
                    _messages.Add(mess.MessageID, mess);
                }
            }

        }

        private void removeMessage(ushort messageID)
        {
            lock (_messages) 
            {
                if (_messages.ContainsKey(messageID))
                {
                    _messages.Remove(messageID);
                }
            }
        }


        public void SetStreamManager(StreamManager strMan)
        {
            _strManager = strMan;
            _running = true;
            Thread thr = new Thread(new ThreadStart(MessageDaemon));
            thr.Start();
        }

        private void OnMessageReceived(MqttMessage mess)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new MqttMessageReceivedEventArgs(mess));
            }
        }

        public void PublishAccepted(ushort messageID, bool accepted)
        {
            // Called if the user accepts a publish
            if (accepted)
            {
                _strManager.SendMessage((MqttMessage)_responses[messageID]);
            }
            else
            {
                _responses.Remove(messageID);
            }
        }

        private List<MqttMessage> GetResendMessages(DateTime now)
        {
            lock (_messages)
            {
                return _messages.Values.Cast<MqttMessage>()
                    .Where(x => (now - new DateTime(x.Timestamp)).TotalMilliseconds >= _resendInterval)
                    .ToList(); // Force enumeration so we can release the lock
            }
        }

        private void MessageDaemon()
        {
            // NOTE: This function should be called in it's own thread
            while (_running)
            {
                var now = DateTime.Now;
                // Check if we should re-send some messages
                foreach (var mess in GetResendMessages(now))
                {
                    mess.Timestamp = now.Ticks;
                    mess.Duplicate = true;
                    try
                    {
                      Log.Write( LogLevel.DEBUG, "Re-Sending - " + mess.MessageID);
                      _strManager.SendMessage(mess);
                    }
                    catch (Exception e) {
                      Log.Write(LogLevel.ERROR, e.ToString());
                      // If we fail for some reason, we will try again another time automatically
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}
