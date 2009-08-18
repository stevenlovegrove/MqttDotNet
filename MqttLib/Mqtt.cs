using System;
using System.Collections.Generic;
using System.Text;
using MqttLib.Core.Messages;
using MqttLib.Core;
using MqttLib;
//using System.Timers;
using System.Threading;
using System.Diagnostics;
using MqttLib.Logger;
using MqttLib.MatchTree;

namespace MqttLib
{
    internal class Mqtt : IMqtt, IMqttShared
    {

        #region Member Variables

        private StreamManager manager = null;
        private QoSManager qosManager = null;
        private IPersistence _store = null;

        private TopicTree<PublishArrivedDelegate> topicTree = null;

        private string _clientID;
        private ushort _keepAlive = 30;
        private ushort messageID = 1;
        private Timer keepAliveTimer = null;

        #endregion

        #region IMqtt Members

        public ushort KeepAliveInterval
        {
          get { return _keepAlive; }
          set { _keepAlive = value; }
        }

        public long ResendInterval {
          get { return qosManager.ResendInterval; }
          set { qosManager.ResendInterval = value; }
        }

        public Mqtt(string connString, string clientID, IPersistence store)
        {
            _store = store;
            qosManager = new QoSManager(_store);
            manager = new StreamManager(connString, qosManager);
            qosManager.MessageReceived += new QoSManager.MessageReceivedDelegate(qosManager_MessageReceived);
            _clientID = clientID;
        }

        void tmrCallback(object args)
        {
          try
          {
            manager.SendMessage(new MqttPingReqMessage());
          }
          catch (Exception e)
          {
            Log.Write(LogLevel.ERROR, e.ToString());
            // We've probably lost a connection. The time will be cancelled when we are
            // notified by the stream manager. In the mean time, just ignore the Exception.
          }
        }

        void qosManager_MessageReceived(object sender, MqttMessageReceivedEventArgs e)
        {
            if (e.Message == null)
            {
                //a null message means we have disconnected from the broker
                OnConnectionLost(new EventArgs());
                return;
            }

            switch (e.Message.MsgType)
            {
                case MessageType.CONNACK:
                    OnConnected(new EventArgs());
                    break;
                case MessageType.DISCONNECT:
                    break;
                case MessageType.PINGREQ:
                    manager.SendMessage(new MqttPingRespMessage());
                    break;
                case MessageType.PUBACK:
                    MqttPubackMessage puback = (MqttPubackMessage)e.Message;
                    OnPublished(new CompleteArgs(puback.AckID));
                    break;
                case MessageType.PUBCOMP:
                    break;
                case MessageType.PUBLISH:
                    MqttPublishMessage m = (MqttPublishMessage)e.Message;
                    OnPublishArrived(m);
                    break;
                case MessageType.PUBREC:
                    break;
                case MessageType.PUBREL:
                    break;
                case MessageType.SUBACK:
                    MqttSubackMessage m1 = (MqttSubackMessage)e.Message;
                    OnSubscribed(new CompleteArgs(m1.AckID));
                    break;
                case MessageType.UNSUBACK:
                    MqttUnsubackMessage m2 = (MqttUnsubackMessage)e.Message;
                    OnUnsubscribed(new CompleteArgs(m2.AckID));
                    break;
                case MessageType.PINGRESP:
                    break;
                case MessageType.UNSUBSCRIBE:
                case MessageType.CONNECT:
                case MessageType.SUBSCRIBE:
                default:
                    throw new Exception("Unsupported Message Type");

            }
        }

        public void Connect()
        {
          DoConnect(new MqttConnectMessage(
            _clientID, _keepAlive, false
          ));
        }

        public void Connect(string willTopic, QoS willQoS, MqttPayload willMsg, bool willRetain)
        {
          DoConnect(new MqttConnectMessage(
            _clientID, _keepAlive, willTopic, willMsg.TrimmedBuffer, willQoS, willRetain, false
          ));
        }

        public void Connect( bool cleanStart )
        {
          DoConnect(new MqttConnectMessage(
            _clientID, _keepAlive, cleanStart
          ));
        }

        public void Connect(string willTopic, QoS willQoS, MqttPayload willMsg, bool willRetain, bool cleanStart )
        {
          DoConnect(new MqttConnectMessage(
            _clientID, _keepAlive, willTopic, willMsg.TrimmedBuffer, willQoS, willRetain, cleanStart
          ));
        }

        private void DoConnect(MqttConnectMessage conmsg)
        {
            try
            {
                manager.Connect();
                manager.SendMessage(conmsg);
                manager.WaitForResponse();
                TimerCallback callback = new TimerCallback(tmrCallback);
                // TODO: Set Keep Alive interval and keepAlive time as property of client
                int keepAliveInterval = 1000 * (_keepAlive / 3);
                keepAliveTimer = new Timer(callback, null, keepAliveInterval, keepAliveInterval);
            }
            catch (Exception e)
            {
                throw new MqttBrokerUnavailableException("Unable to connect to the broker", e);
            }
        }

        public void Disconnect()
        {
            manager.SendMessage(new MqttDisconnectMessage());
            if (keepAliveTimer != null)
            {
              keepAliveTimer.Dispose();
              keepAliveTimer = null;
            }
            manager.Disconnect();
            
        }

        public int Publish(string topic, MqttPayload payload, QoS qos, bool retained)
        {
            if (manager.IsConnected)
            {
                ushort messID = MessageID;
                manager.SendMessage(new MqttPublishMessage(messID, topic, payload.TrimmedBuffer, qos, retained));
                return messID;
            }
            else
            {
                throw new MqttNotConnectedException("You need to connect to a broker before trying to Publish");
            }
        }

        public int Publish(MqttParcel parcel)
        {
          return Publish(parcel.Topic, parcel.Payload, parcel.Qos, parcel.Retained);
        }

        public int Subscribe(Subscription[] subscriptions)
        {
            if (manager.IsConnected)
            {
                ushort messID = MessageID;
                manager.SendMessage(new MqttSubscribeMessage(messID, subscriptions));
                return messID;
            }
            else
            {
                throw new MqttNotConnectedException("You need to connect to a broker before trying to Publish");
            }
        }

        public int Subscribe(Subscription subscription)
        {
            return Subscribe(new Subscription[] { subscription });
        }

        public int Subscribe(string topic, QoS qos)
        {
            return Subscribe(new Subscription(topic, qos));
        }

        public int Unsubscribe(string[] topics)
        {
            if (manager.IsConnected)
            {
                ushort messID = MessageID;
                manager.SendMessage(new MqttUnsubscribeMessage(messID, topics));
                return messID;
            }
            else
            {
                throw new MqttNotConnectedException("You need to connect to a broker before trying to Publish");
            }
        }

        public bool IsConnected
        {
            get
            {
                return manager.IsConnected;
            }
        }

        public event PublishArrivedDelegate PublishArrived;

        public event CompleteDelegate Published;

        public event CompleteDelegate Subscribed;

        public event CompleteDelegate Unsubscribed;

        public event ConnectionDelegate ConnectionLost;

        public event ConnectionDelegate Connected;

        #endregion

        #region IMqttSharedSubscriber Members

        public void Subscribe(Subscription subscription, PublishArrivedDelegate subscriber)
        {
          if (topicTree == null)
          {
            topicTree = new TopicTree<PublishArrivedDelegate>();
          }

          topicTree.Add( subscription.Topic, subscriber);

          // TODO: Check if we're already subscribed.
          Subscribe(subscription);
        }

        public void Unsubscribe(string topic, PublishArrivedDelegate subscriber)
        {
          topicTree.Remove( topic, subscriber);

          // TODO: Check if this is the last subscriber
          Unsubscribe(new string[] { topic } );
        }

        #endregion
      
        #region Event Raising functions

        protected void OnPublishArrived(MqttPublishMessage m)
        {
            bool accepted = false;

            if (PublishArrived != null)
            {
                PublishArrivedArgs e = new PublishArrivedArgs(m.Topic, m.Payload, m.Retained, m.QualityOfService);
                try
                {
                  accepted |= PublishArrived(this, e);
                }
                catch(Exception ex)
                {
                  MqttLib.Logger.Log.Write(LogLevel.ERROR, "MqttLib: Uncaught exception from user delegate: " + ex.ToString());
                }
            }

            if (topicTree != null)
            {
                PublishArrivedArgs e = new PublishArrivedArgs(m.Topic, m.Payload, m.Retained, m.QualityOfService);
                List<PublishArrivedDelegate> subscribers = topicTree.CollectMatches(new Topic(m.Topic));
                foreach (PublishArrivedDelegate pad in subscribers)
                {
                  try
                  {
                    accepted |= pad(this, e);
                  }
                  catch (Exception ex)
                  {
                    MqttLib.Logger.Log.Write(LogLevel.ERROR, "MqttLib: Uncaught exception from user delegate: " + ex.ToString());
                  }
                }
            }

            if (m.QualityOfService > QoS.BestEfforts)
            {
              qosManager.PublishAccepted(m.MessageID, accepted);
            }

        }

        protected void OnPublished(CompleteArgs e)
        {
            if (Published != null)
            {
              try
              {
                Published(this, e);
              }
              catch (Exception ex)
              {
                MqttLib.Logger.Log.Write(LogLevel.ERROR, "MqttLib: Uncaught exception from user delegate: " + ex.ToString());
              }
            }
        }

        protected void OnSubscribed(CompleteArgs e)
        {
            if (Subscribed != null)
            {
              try
              {
                Subscribed(this, e);
              }
              catch (Exception ex)
              {
                MqttLib.Logger.Log.Write(LogLevel.ERROR, "MqttLib: Uncaught exception from user delegate: " + ex.ToString());
              }
            }
        }

        protected void OnUnsubscribed(CompleteArgs e)
        {
            if (Unsubscribed != null)
            {
              try {
                Unsubscribed(this, e);
              }
              catch (Exception ex)
              {
                MqttLib.Logger.Log.Write(LogLevel.ERROR, "MqttLib: Uncaught exception from user delegate: " + ex.ToString());
              }
            }
        }

        protected void OnConnectionLost(EventArgs e)
        {
          if (keepAliveTimer != null)
          {
            keepAliveTimer.Dispose();
            keepAliveTimer = null;
          }

          if (ConnectionLost != null)
          {
            try
            {
              ConnectionLost(this, e);
            }
            catch (Exception ex)
            {
              MqttLib.Logger.Log.Write(LogLevel.ERROR, "MqttLib: Uncaught exception from user delegate: " + ex.ToString());
            }
          }
        }

        protected void OnConnected(EventArgs e)
        {
            if (Connected != null)
            {
              try
              {
                Connected(this, e);
              }
              catch (Exception ex)
              {
                MqttLib.Logger.Log.Write(LogLevel.ERROR, "MqttLib: Uncaught exception from user delegate: " + ex.ToString());
              }
            }
        }



        #endregion

        private ushort MessageID
        {
            get
            {
                return messageID++;
            }
        }

      }
}
