using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.MatchTree
{
  public class TopicTree<T>
  {
    #region Member Variables

    /// <summary>
    /// Root node to which levels are attached
    /// </summary>
    internal TopicNode<T> rootNode = new TopicNode<T>(".");

    #endregion //Member Variables

    /// <summary>
    /// Associate <code>value</code> with <code>topic</code> wildcard
    /// </summary>
    /// <param name="topic">topic wildcard match sequence</param>
    /// <param name="value">Associated value</param>
    public void Add(Topic topic, T value)
    {
      rootNode.AddTopicValue(topic, 0, value);
    }

    /// <summary>
    /// Collect Matches for <code>topic</code>.
    /// </summary>
    /// <param name="topic">The Full topic to match against</param>
    /// <returns>A list containing all values that match <code>topic</code>.</returns>
    public List<T> CollectMatches(Topic topic)
    {
      List<T> matches = new List<T>();
      rootNode.CollectMatches( topic, 0, matches );
      return matches;
    }

    /// <summary>
    /// Remove values attributed to <code>topic</code> unexpanded. i.e no wildcard matching is used.
    /// </summary>
    /// <param name="topic">Topic to Match</param>
    /// <param name="value">Value to Remove</param>
    public void Remove(Topic topic, T value)
    {
      rootNode.Remove(topic, 0, value);
    }

    /// <summary>
    /// Remove all attached values equal to <code>value</code>
    /// </summary>
    /// <param name="value">Value to Remove</param>
    public void RemoveAll(T value)
    {
      rootNode.RemoveAll(value);
    }

    /// <summary>
    /// Remove all values from the Topic Tree
    /// </summary>
    public void RemoveAll()
    {
      rootNode = new TopicNode<T>(".");
    }

  }
}
