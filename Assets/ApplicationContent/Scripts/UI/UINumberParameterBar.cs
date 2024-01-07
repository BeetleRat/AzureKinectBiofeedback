using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// <para>Class displays the value of a numeric parameter on the UI. Display format: "Parameter name: Value"</para>
/// </summary>
public class UINumberParameterBar : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private string _parameterName;
    [Range(0, 6)] [SerializeField] private int _decimalPoint;

    [SerializeField] private bool _barVisibility;

    /// <summary>
    /// <para>Parameter value displayed in UI each frame</para>
    /// </summary>
    protected float parameterValue = 0;

    protected virtual void Update()
    {
        if (_barVisibility)
        {
            _text.text = _parameterName + ": " + Math.Round(parameterValue, _decimalPoint);
        }
        else
        {
            _text.text = "";
        }
    }

    /// <summary>
    /// <para>Set bar visibility</para>
    /// </summary>
    /// <param name="visible">Is bar visible</param>
    public void SetVisible(bool visible)
    {
        _barVisibility = visible;
    }

    /// <summary>
    /// <para>Set showed parameter name</para>
    /// </summary>
    /// <param name="parameterName">The parameter name</param>
    public void SetParameterName(string parameterName)
    {
        _parameterName = parameterName;
    }

    /// <summary>
    /// <para>Set showed parameter value</para>
    /// </summary>
    /// <param name="parameterValue">The parameter value</param>
    public virtual void SetParameterValue(float parameterValue)
    {
        this.parameterValue = parameterValue;
    }

    /// <summary>
    /// <para>Set the color of the bar text</para>
    /// </summary>
    /// <param name="newColor">The new text color</param>
    public void SetColor(Color newColor)
    {
        _text.color = newColor;
    }
}