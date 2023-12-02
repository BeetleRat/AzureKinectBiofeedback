using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// <para>A class that triggers events associated with a biofeedback device</para>
/// </summary>
public class EventMessageListener : MonoBehaviour
{
    /// <summary>
    /// <para>Event triggered when the connection status of the device changes</para>
    /// </summary>
    public event UnityAction<bool> ConnectionChanged;

    /// <summary>
    /// <para>Event triggered when a message is arrived from the device</para>
    /// </summary>
    public event UnityAction<string> MessageArrived;


    private const string MESSAGE_ARRIVED_PREFIX = "Message arrived: ";

    // Invoked when a line of data is received from the serial device.
    private void OnMessageArrived(string msg)
    {
        Debug.Log(MESSAGE_ARRIVED_PREFIX + msg);
        MessageArrived?.Invoke(msg);
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    private void OnConnectionEvent(bool success)
    {
        ConnectionChanged?.Invoke(success);
    }
}