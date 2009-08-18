using System;
using System.Collections.Generic;
using System.Text;

namespace MqttLib.MatchTree
{
  /// <summary>
  /// Represents a level in the Topic Hierarchy tree.
  /// </summary>
  internal class TopicNode<T>
  {

    #region Member Variables.

    private string nodevalue;

    /// <summary>
    /// Children in the topic hierarchy
    /// </summary>
    protected Dictionary<string, TopicNode<T>> children = null;

    /// <summary>
    /// The associated values of the TopicNode.
    /// </summary>
    protected List<T> values = new List<T>();

    #endregion // Member Variables

    #region Fields

    /// <summary>
    /// The value for the level in the hierarchy
    /// </summary>
    public string Nodevalue
    {
      get { return nodevalue; }
    }

    /// <summary>
    /// The associated values of the TopicNode.
    /// </summary>
    public List<T> Values
    {
      get { return values; }
    }

    #endregion // Fields

    /// <summary>Construct TopicNode</summary>
    /// <param name="nodevalue">The value for this level of the topic hierarchy</param>
    public TopicNode(string nodevalue )
    {
      this.nodevalue = nodevalue;
    }

    /// <summary>Construct TopicNode</summary>
    /// <param name="nodevalue">The value for this level of the topic hierarchy</param>
    /// <param name="attachedvalue">Value attached to this node</param>
    public TopicNode(string nodevalue, T attachedvalue)
    {
      this.nodevalue = nodevalue;
      values = new List<T>();
      values.Add(attachedvalue);
    }

    /// <summary>
    /// Add Topic Value to Node given current location in the topic parsing procedure.
    /// </summary>
    /// <param name="t">Topic which holds <code>value</code></param>
    /// <param name="level">Current Topic Level being parse</param>
    /// <param name="value">Value to store against topic</param>
    internal void AddTopicValue(Topic t, uint level, T value)
    {
      if (level == t.Levels.Length)
      {
        // We should add this value to this nodes values if it isn't already.
        if (!values.Contains(value))
        {
          values.Add(value);
        }
      }
      else
      {
        TopicNode<T> child;

        if (children == null)
        {
          children = new Dictionary<string, TopicNode<T>>();
        }

        if ( !children.TryGetValue(t.Levels[level], out child))
        {
          // subnode doesn't exist already
          child = new TopicNode<T>(t.Levels[level]);
          children.Add(t.Levels[level], child);
        }

        child.AddTopicValue(t, level + 1, value);
      }
    }

    /// <summary>
    /// Recursively collect values from this node and sub-children that match the given topic.
    /// </summary>
    /// <param name="topic">Topic to Match</param>
    /// <param name="level">Current level being matched against</param>
    /// <param name="matches">List containing matches so far.</param>
    internal void CollectMatches(Topic topic, uint level, List<T> matches)
    {
      if (level >= topic.Levels.Length)
      {
        foreach (T value in values)
        {
          if (!matches.Contains(value))
          {
            matches.Add(value);
          }
        }
      }else{

        if( nodevalue.Equals(Topic.TOPIC_ANY_MANY.ToString()) )
        {
          CollectMatches(topic, level + 1, matches);
        }

        if( children != null )
        {
          string levelstr = topic.Levels[level];
          bool collectmany = topic.Levels[level].Equals(Topic.TOPIC_ANY_MANY.ToString());

          foreach (KeyValuePair<string, TopicNode<T>> pair in children)
          {
            if (pair.Value.Matches(levelstr))
            {
              pair.Value.CollectMatches(topic, level + 1, matches);
              if (collectmany)
              {
                pair.Value.CollectMatches(topic, level, matches);
              }
            }
          }
        }

      }

    }

    /// <param name="levelstring">level hierarchy to match against</param>
    /// <returns><code>true</code>iff this node successfully matches <code>levelstring</code> given wildcard charactors</returns>
    internal bool Matches(string levelstring)
    {
      return
        levelstring.Equals(nodevalue) ||
        nodevalue.Equals(Topic.TOPIC_ANY_ONE.ToString()) ||
        nodevalue.Equals(Topic.TOPIC_ANY_MANY.ToString()) ||
        levelstring.Equals(Topic.TOPIC_ANY_ONE.ToString()) ||
        levelstring.Equals(Topic.TOPIC_ANY_MANY.ToString());
    }

    /// <summary>
    /// Remove all attached values equal to <code>value</code>
    /// </summary>
    /// <param name="value">Value to remove</param>
    internal void RemoveAll(T value)
    {
      values.Remove(value);
      if (children != null)
      {
        foreach (TopicNode<T> child in children.Values)
        {
          child.RemoveAll(value);
          // TODO: Remove useless children
        }
      }
    }

    /// <summary>
    /// Remove values attributed to <code>topic</code> unexpanded. i.e no wildcard matching is used.
    /// </summary>
    /// <param name="topic">Topic to match</param>
    /// <param name="value">Value to remove</param>
    /// <param name="level">Current level being matched against</param>
    internal void Remove(Topic topic, uint level, T value)
    {
      if (level >= topic.Levels.Length)
      {
        // Remove from this node
        values.Remove(value);
      }
      else
      {
        if (children != null) {
          foreach (KeyValuePair<string, TopicNode<T>> pair in children)
          {
            if (pair.Value.nodevalue.Equals(topic.Levels[level])) {
              pair.Value.Remove(topic, level + 1, value);
            }
          }
        }
      }

    }

  }
}
