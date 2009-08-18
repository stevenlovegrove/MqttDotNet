using System;
namespace MqttLib.Core.Messages
{
    public interface IPersitentMessage
    {
        ushort MessageID { get; }
        MessageType MsgType { get; }
        QoS QualityOfService { get; }
        bool Retained { get; }
        void Serialise(System.IO.Stream str);
    }
}
