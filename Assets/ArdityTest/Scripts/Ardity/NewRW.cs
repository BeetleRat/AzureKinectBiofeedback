/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using System;
using UnityEngine;
using UnityEngine.Serialization;

/**
 * Sample for reading using polling by yourself, and writing too.
 */
public class NewRW : MonoBehaviour
{
    private const string CONNECTED_MESSAGE = "Connection established";
    private const string CONNECTED_FAILED_MESSAGE = "Connection attempt failed or disconnection detected";

    [SerializeField] private bool _send = false; //debug
    [SerializeField] private SerialController _serialController;
    public static bool connected = false;
    public static string messageConnected = "";

    // Initialization
    void Start()
    {
        Debug.Log("Press 1 to execute some actions");
    }

    // Executed each frame
    void Update()
    {
        //---------------------------------------------------------------------
        // Send data
        //---------------------------------------------------------------------

        // If you press one of these keys send it to the serial device. A
        // sample serial device that accepts this input is given in the README.
        if (_send && Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Sending 1");
            _serialController.SendSerialMessage("1");
        }

        //---------------------------------------------------------------------
        // Receive data
        //---------------------------------------------------------------------
        connected = NewSampleMessageListener.establishedConnection;

        string message = NewSampleMessageListener.arrivedMessage;

        if (message == null || message == CONNECTED_MESSAGE || message == CONNECTED_FAILED_MESSAGE)
            return;

        messageConnected = message;
    }
}