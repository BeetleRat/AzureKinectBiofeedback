using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// <para>Implementing a <see cref="AbstractBiofeedbackManager"/> through heart rate</para>
/// </summary>
public class PulseManager : AbstractBiofeedbackManager
{
    /// <summary>
    /// <inheritdoc cref="AbstractBiofeedbackManager.PulseConditionChange"/>
    /// </summary>
    public override event UnityAction<PulseCondition> PulseConditionChange;
    
    private int pulse = 80;

    protected override void Start()
    {
        base.Start();
        pulseReceiver.PulseReceived += SetPulse;
    }


    private void OnDestroy()
    {
        pulseReceiver.PulseReceived -= SetPulse;
    }

    private void SetPulse(int newPulse)
    {
        if (newPulse != pulse)
        {
            pulse = newPulse;
            RecalculateCondition();
            _debugUiBar?.SetParameterValue(pulse);
        }
    }

    private void RecalculateCondition()
    {
        if (_pulseRateConditions.Count == 0)
        {
            Debug.LogError($"[{name}]: Pulse Rate Conditions List is empty.");
            return;
        }

        for (int i = 0; i < _pulseRateConditions.Count; i++)
        {
            PulseRateCondition currentPulseRateCondition = _pulseRateConditions[i];
            if (pulse < currentPulseRateCondition.Rate)
            {
                SetNewPulseConditionFromList(i);
                return;
            }
        }

        SetNewPulseConditionFromList(_pulseRateConditions.Count - 1);
    }

    private void SetNewPulseConditionFromList(int elementIndex)
    {
        PulseRateCondition selectedPulseRateCondition = _pulseRateConditions[elementIndex];
        PulseCondition newCondition = selectedPulseRateCondition.Condition;
        _debugUiBar?.SetColor(selectedPulseRateCondition.UIBarColor);
        if (pulseCondition != newCondition)
        {
            pulseCondition = newCondition;
            PulseConditionChange?.Invoke(pulseCondition);
        }
    }
}