using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// <para>Button object component. When a <see cref="PlayerBodypart"/> touches this object, the button press action will be triggered</para>
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class ObjectButton : MonoBehaviour
{
    /// <summary>
    /// <para>Action triggered when a <see cref="PlayerBodypart"/> touches button object</para>
    /// </summary>
    public event UnityAction<ObjectButton> ButtonPressed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerBodypart player))
        {
            ButtonPressed?.Invoke(this);
        }
    }
}