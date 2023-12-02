using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// <para>Implementing a <see cref="AbstractBiofeedbackManager"/> through heart rate acceleration</para>
/// </summary>
public class PulseAccelerationManager : AbstractBiofeedbackManager
{
    /// <summary>
    /// <inheritdoc cref="AbstractBiofeedbackManager.PulseConditionChange"/>
    /// </summary>
    public override event UnityAction<PulseCondition> PulseConditionChange;
    
    private float acceleration = 80;

    protected override void Start()
    {
        base.Start();
        pulseReceiver.AccelerationCalculated += SetAcceleration;
    }

    private void OnDestroy()
    {
        pulseReceiver.AccelerationCalculated -= SetAcceleration;
    }

    private void SetAcceleration(float acceleration)
    {
        if (acceleration != this.acceleration)
        {
            this.acceleration = acceleration;
            RecalculateCondition();
            _debugUiBar?.SetParameterValue(this.acceleration);
        }
    }

    private void RecalculateCondition()
    {
        if (_pulseRateConditions.Count == 0)
        {
            Debug.LogError($"[{name}]: Pulse Rate Conditions List is empty.");
            return;
        }

        // TODO Определить как изменять состояние пульса в зависимости от ускарения
        for (int i = 0; i < _pulseRateConditions.Count; i++)
        {
            if (_pulseRateConditions[i].Condition == PulseCondition.NORMAL)
            {
                SetNewPulseConditionFromList(i);
                return;
            }
        }
    }

    private void SetNewPulseConditionFromList(int elementIndex)
    {
        PulseRateCondition selectedPulseAccelerationCondition = _pulseRateConditions[elementIndex];
        PulseCondition newCondition = selectedPulseAccelerationCondition.Condition;
        _debugUiBar?.SetColor(selectedPulseAccelerationCondition.UIBarColor);
        if (pulseCondition != newCondition)
        {
            pulseCondition = newCondition;
            PulseConditionChange?.Invoke(pulseCondition);
        }
    }
}