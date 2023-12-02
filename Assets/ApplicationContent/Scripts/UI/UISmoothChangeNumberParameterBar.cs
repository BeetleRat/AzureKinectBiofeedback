using System;
using UnityEngine;

/// <summary>
/// <para>
///<inheritdoc cref="UINumberParameterBar"/>
/// If the value of a parameter has changed, the value on the UI will change from the previous value to the new value at the selected speed
/// </para>
/// </summary>
public class UISmoothChangeNumberParameterBar : UINumberParameterBar
{
    [Range(1, 1000)] [SerializeField] private int _changeParameterSpeed = 10;

    private float currentValue = 0;
    private float parameterSum;

    protected override void Update()
    {
        if (Math.Abs(currentValue - parameterValue) > 0)
        {
            parameterSum += Time.deltaTime * (currentValue - parameterValue) * _changeParameterSpeed;
            parameterValue = parameterSum;
        }

        base.Update();
    }

    /// <summary>
    /// <inheritdoc cref="UINumberParameterBar.SetParameterValue"/>
    /// </summary>
    /// <param name="parameterValue"><inheritdoc cref="UINumberParameterBar.SetParameterValue"/></param>
    public override void SetParameterValue(float parameterValue)
    {
        currentValue = parameterValue;
    }

    /// <summary>
    /// <para>Add number value current. If you want to subtract from the current value just add a negative number</para>
    /// </summary>
    /// <param name="addedValue">The added number value</param>
    public void AddValue(float addedValue)
    {
        parameterSum = parameterValue;
        currentValue += addedValue;
    }
}