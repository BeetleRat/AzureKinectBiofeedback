using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <para>Script for displaying one health point in UI</para>
/// </summary>
public class UIHealthPoint : MonoBehaviour
{
    [SerializeField] private Image _activeHeart;
    [SerializeField] private Image _notActiveHeart;
    [SerializeField] private bool _isActive;

    private void Start()
    {
        _activeHeart.gameObject.SetActive(_isActive);
        _notActiveHeart.gameObject.SetActive(!_isActive);
    }

    /// <summary>
    /// Set active state for health point
    /// </summary>
    /// <param name="isActive">Is health point active</param>
    public void SetActive(bool isActive)
    {
        _isActive = isActive;

        _activeHeart.gameObject.SetActive(_isActive);
        _notActiveHeart.gameObject.SetActive(!_isActive);
    }
}