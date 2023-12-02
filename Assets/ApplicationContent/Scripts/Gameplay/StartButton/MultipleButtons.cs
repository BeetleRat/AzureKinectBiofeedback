using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// <para>Script that triggers a multiple <see cref="ObjectButton"/> press action</para>
/// </summary>
public class MultipleButtons : MonoBehaviour
{
    /// <summary>
    /// <para>Simultaneous <see cref="ObjectButton"/> press action</para>
    /// </summary>
    public event UnityAction ButtonsPressed;

    [SerializeField] private ObjectButton[] _objectButtons;

    private List<ObjectButton> activeStartButtons;
    private bool isButtonsHide = false;
    private float hideDuration = 1;

    private void Start()
    {
        activeStartButtons = new List<ObjectButton>();
        foreach (ObjectButton startButton in _objectButtons)
        {
            startButton.ButtonPressed += ActivateStartButton;
        }
    }

    private void OnDestroy()
    {
        foreach (ObjectButton startButton in _objectButtons)
        {
            startButton.ButtonPressed -= ActivateStartButton;
        }
    }

    private void ActivateStartButton(ObjectButton objectButton)
    {
        if (isButtonsHide || activeStartButtons.Contains(objectButton))
        {
            return;
        }

        activeStartButtons.Add(objectButton);
        objectButton.transform.DOScale(0.7f, 1);

        if (activeStartButtons.Count >= _objectButtons.Length)
        {
            ButtonsPressed?.Invoke();
        }
    }

    /// <summary>
    /// <para>Show buttons if they are hidden</para>
    /// </summary>
    public void Show()
    {
        if (isButtonsHide)
        {
            transform.DOMoveY(0, hideDuration);
            foreach (ObjectButton startButton in _objectButtons)
            {
                startButton.transform.DOScale(1f, 1);
            }

            isButtonsHide = false;
            activeStartButtons.Clear();
        }
    }

    /// <summary>
    /// <para>Hide buttons if they are not hide</para>
    /// </summary>
    public void Hide()
    {
        if (!isButtonsHide)
        {
            isButtonsHide = true;
            transform.DOMoveY(-3, hideDuration);
        }

        activeStartButtons.Clear();
    }
}