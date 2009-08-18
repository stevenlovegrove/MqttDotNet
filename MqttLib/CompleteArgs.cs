using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib
{
  public class CompleteArgs
  {

    #region Member Variables

    private int _messageID;

    #endregion

    #region Constructors

    public CompleteArgs(int messageID)
    {
      _messageID = messageID;
    }

    #endregion

    #region Properties

    /// <summary>
    /// ID of the message
    /// </summary>
    public int MessageID
    {
      get
      {
        return _messageID;
      }
    }

    #endregion
  }
}
