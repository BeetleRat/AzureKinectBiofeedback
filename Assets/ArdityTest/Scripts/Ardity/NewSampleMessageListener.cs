/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using System;
using UnityEngine;

/**
 * When creating your message listeners you need to implement these two methods:
 *  - OnMessageArrived
 *  - OnConnectionEvent
 */
public class NewSampleMessageListener : MonoBehaviour
{
    private const string MESSAGE_ARRIVED_PREFIX = "Message arrived: ";
    
    public static string arrivedMessage;
    public static bool establishedConnection = false;
 

    // Invoked when a line of data is received from the serial device.
    private void OnMessageArrived(string msg)
    {
        Debug.Log(MESSAGE_ARRIVED_PREFIX + msg);
        arrivedMessage = msg;
    }

    // Invoked when a connect/disconnect event occurs. The parameter 'success'
    // will be 'true' upon connection, and 'false' upon disconnection or
    // failure to connect.
    private void OnConnectionEvent(bool success)
    {
        if (success)
        {
            Debug.Log("Connection established");
            establishedConnection = true;
        }
        else
        {
            Debug.Log("Connection attempt failed or disconnection detected");
            establishedConnection = false;
        }
    }
}
