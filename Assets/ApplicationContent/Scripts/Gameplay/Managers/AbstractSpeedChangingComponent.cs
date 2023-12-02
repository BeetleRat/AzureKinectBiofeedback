using System;using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// <para>Enumeration of game speed states</para>
/// </summary>
[Serializable]
public enum Speed
{
    STOP,
    SLOW,
    NORMAL,
    FAST
}

/// <summary>
/// <para>Abstract class of component that changes its behavior depending on current game speed</para>
/// </summary>
public abstract class AbstractSpeedChangingComponent : MonoBehaviour
{
    /// <summary>
    /// <para>Change the behavior of this component according to selected <see cref="Speed"/></para>
    /// </summary>
    /// <param name="speed">the selected <see cref="Speed"/></param>
    public abstract void ChangeSpeed(Speed speed);

    protected void LogNoSpeedSettingsFor(Speed speed)
    {
        Debug.LogError($"[{name}]: No speed settings are specified for speed {Enum.GetName(typeof(Speed), speed)}.");
    }
}