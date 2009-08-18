using System;
using MqttLib;

/// <summary>
/// Delegate represents end-point for receipt of messages
/// </summary>
/// <param name="sender">Object responsible for invoking the delegate</param>
/// <param name="e">Arguments containing Message body and other detains</param>
/// <returns>return true to signify receipt of the message</returns>
public delegate bool PublishArrivedDelegate(object sender, PublishArrivedArgs e);

/// <summary>
/// Delegate represents completion of some task, such as message delivery.
/// </summary>
/// <param name="sender">Object responsible for invoking the delegate</param>
/// <param name="e">Arguments contain a unique ID for the completed task. This
/// ID will have been provided on initiating the task</param>
public delegate void CompleteDelegate(object sender, CompleteArgs e);

/// <summary>
/// Delegate represents a change in connection status.
/// </summary>
/// <param name="sender">Object responsible for invoking the delegate</param>
/// <param name="e"></param>
public delegate void ConnectionDelegate(object sender, EventArgs e);
