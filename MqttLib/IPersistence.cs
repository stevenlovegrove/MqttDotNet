using System;
using System.Text;
using MqttLib.Core.Messages;

namespace MqttLib
{
    /// <summary>
    /// The persistent message store to use
    /// </summary>
    public enum MessageStore
    {
        Sent,
        Received
    }

    public interface IPersistence
    {
        /// <summary>
        /// Add a message to the persistent store
        /// </summary>
        /// <param name="m">Message</param>
        /// <param name="store">Persistent message store</param>
        void AddMessage(IPersitentMessage m, MessageStore store);

        /// <summary>
        /// Delete a message from the persistent message store
        /// </summary>
        /// <param name="messageID">Unique ID of the message</param>
        /// <param name="store">Persistent message store</param>
        void DeleteMessage(int messageID, MessageStore store);

        /// <summary>
        /// Update a message in the sent persistent message store
        /// </summary>
        /// <param name="m">Message</param>
        void UpdateSentMessage(IPersitentMessage m);

        /// <summary>
        /// Get all messages stored in the given persistent store
        /// </summary>
        /// <param name="store">persistent message store</param>
        /// <returns>Array od messages recevied from the persistent store</returns>
        IPersitentMessage[] GetAllMessages(MessageStore store);

        /// <summary>
        /// Inititlise the persistent store
        /// </summary>
        /// <param name="clientID">Unique ID of the client</param>
        /// <param name="connectionID">Unique ID for the connection</param>
        void Open(string clientID, string connectionID);

        /// <summary>
        /// Reset the persistent store to an empty state
        /// </summary>
        void Reset();

        /// <summary>
        /// Close the persistent store, this will be called when a client application disconnects from the broker
        /// </summary>
        void Close();
        
    }
}
