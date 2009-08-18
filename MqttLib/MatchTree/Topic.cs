using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.MatchTree
{
  /// <summary>
  /// Class representing the Topic space
  /// </summary>
  public class Topic
  {
    #region Class Constants

    /// <summary>
    /// Topic Hierarchy Wildcard representing any value for the level.
    /// </summary>
    public const char TOPIC_ANY_ONE = '+';

    /// <summary>
    /// Topic Hierarchy Wildcard representing any value for any number of levels including 0.
    /// </summary>
    public const char TOPIC_ANY_MANY = '#';

    /// <summary>
    /// Topic Hierarchy Seperator
    /// </summary>
    public const char TOPIC_SEPERATOR = '/';

    #endregion

    private string[] levels;
    public string[] Levels
    {
      get { return levels; }
    }

    /// <summary>
    /// Construct Topic from Topic string
    /// </summary>
    /// <param name="topicname">String representation of the topic hierarchy.</param>
    public Topic(string topicname)
    {
      levels = topicname.Split(TOPIC_SEPERATOR);
    }

    #region Cast Operators

    public static implicit operator Topic(string str)
    {
      return new Topic(str);
    }

    #endregion

  }
}
