using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// <para>A component that requests, receives and processes the pulse from a biofeedback device</para>
/// If a biofeedback device is not available, this component may use placeholder
/// </summary>
[RequireComponent(typeof(EventMessageListener))]
public class PulseReceiver : MonoBehaviour
{
    /// <summary>
    /// <para>Event triggered when a heart rate value is received from a device or placeholder</para>
    /// </summary>
    public event UnityAction<int> PulseReceived;

    /// <summary>
    /// <para>Event triggered when heart rate acceleration is recalculated</para>
    /// </summary>
    public event UnityAction<float> AccelerationCalculated;

    private const string PULSE_PREFIX = "pulse=";
    private const string PULSE_REQUEST_MESSAGE = "1";
    private const int PULSE_NOT_RECEIVED = -1;

    [Tooltip("Whether to write debug information to the console")] 
    [SerializeField] private bool _debug = false;

    [SerializeField] private SerialController _serialController;

    [Tooltip("Frequency of requesting a heart rate value from the device")] [Min(0.000001f)] 
    [SerializeField] private float _pollingRateSeconds;

    [Tooltip("Device polling period for which heart rate acceleration will be recalculated")] [Min(1)] 
    [SerializeField] private int _recalculatingCyclePeriod;

    [SerializeField] private bool _usePlaceholder;
    [Range(1, 150)] [SerializeField] private int _placeholderPulse;
    [SerializeField] private float _placeholderAcceleration;

    private bool connected = false;
    private float timeSinceLastPolling = 0;
    private EventMessageListener eventMessageListener;
    private int currentRecalculatingCycle;
    private int firstPeriodPulse;
    private int lastPeriodPulse;

    private void Start()
    {
        eventMessageListener = GetComponent<EventMessageListener>();
        currentRecalculatingCycle = 0;
        firstPeriodPulse = 0;
        eventMessageListener.MessageArrived += GetMessageFromSerial;
        eventMessageListener.ConnectionChanged += SetConnectionWithSerial;
    }

    private void FixedUpdate()
    {
        timeSinceLastPolling += Time.deltaTime;
        if (timeSinceLastPolling > _pollingRateSeconds)
        {
            if (_usePlaceholder)
            {
                GetBiofeedbackFromPlaceholder();
            }
            else
            {
                SendMessageToSerialController();
            }

            timeSinceLastPolling = 0;
        }
    }

    private void OnDestroy()
    {
        eventMessageListener.MessageArrived -= GetMessageFromSerial;
        eventMessageListener.ConnectionChanged -= SetConnectionWithSerial;
    }

    private void SetConnectionWithSerial(bool isConnected)
    {
        connected = isConnected;

        if (connected)
        {
            DebugLog("Connection established");
            SendMessageToSerialController();
        }
        else
        {
            DebugLog("Connection attempt failed or disconnection detected");
        }
    }

    private void GetMessageFromSerial(string msg)
    {
        string messageFromBiofeedback = msg;
        DebugLog("Serial controller returned message: " + messageFromBiofeedback);

        int pulseNumber = GetPulseFromMessage(messageFromBiofeedback);
        CalculateAcceleration(pulseNumber);
    }

    private void GetBiofeedbackFromPlaceholder()
    {
        string pulseMessage = PULSE_PREFIX + _placeholderPulse;

        int pulseNumber = GetPulseFromMessage(pulseMessage);
        CreatePlaceholderAcceleration(_placeholderAcceleration);
    }


    private void SendMessageToSerialController()
    {
        if (!connected)
        {
            PulseReceived?.Invoke(PULSE_NOT_RECEIVED);
            DebugErr($"Failed to retrieve data from the device. The device is not responding. The heart rate value is set to {PULSE_NOT_RECEIVED.ToString()}");
        }
        _serialController.SendSerialMessage(PULSE_REQUEST_MESSAGE);
        DebugLog("Send message to serial controller: " + PULSE_REQUEST_MESSAGE);
    }

    private int GetPulseFromMessage(string messageFromBiofeedback)
    {
        if (messageFromBiofeedback != null && messageFromBiofeedback.Contains(PULSE_PREFIX))
        {
            string pulseNumberAsString = messageFromBiofeedback.Replace(PULSE_PREFIX, "");
            int receivedPulseNumber = int.Parse(pulseNumberAsString);
            PulseReceived?.Invoke(receivedPulseNumber);
            return receivedPulseNumber;
        }

        return 0;
    }

    private void CalculateAcceleration(int pulse)
    {
        // TODO Уточнить формулу получения ускорения пульса
        float acceleration = 0.0f;
        currentRecalculatingCycle++;
        if (currentRecalculatingCycle >= _recalculatingCyclePeriod)
        {
            currentRecalculatingCycle = 0;
            int pulseRange = pulse - firstPeriodPulse;
            acceleration = pulseRange / (_pollingRateSeconds * _recalculatingCyclePeriod);
            DebugLog($"Acceleration was recalculated: {acceleration} = {pulseRange.ToString()}/({_pollingRateSeconds.ToString()}*{_recalculatingCyclePeriod.ToString()})");

            AccelerationCalculated?.Invoke(acceleration);
            firstPeriodPulse = pulse;
        }
    }

    private void CreatePlaceholderAcceleration(float acceleration)
    {
        currentRecalculatingCycle++;
        if (currentRecalculatingCycle >= _recalculatingCyclePeriod)
        {
            currentRecalculatingCycle = 0;
            AccelerationCalculated?.Invoke(acceleration);
        }
    }

    private void DebugLog(string message)
    {
        if (_debug)
        {
            Debug.Log($"[{name}]: " + message);
        }
    }
    
    private void DebugErr(string message)
    {
        if (_debug)
        {
            Debug.LogError($"[{name}]: " + message);
        }
    }
}