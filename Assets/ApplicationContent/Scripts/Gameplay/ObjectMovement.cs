using UnityEngine;

/// <summary>
/// <para>A component used to specify the trajectory of an object</para>
/// </summary>
[ExecuteAlways]
public class ObjectMovement : MonoBehaviour
{
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;

    private void Start()
    {
        if (Application.IsPlaying(this))
        {
            DestroyChildren(_startPoint);
            DestroyChildren(_endPoint);
        }
    }

    private void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void Update()
    {
        if (_startPoint && _endPoint && !Application.isPlaying)
        {
            Color drawColor = _endPoint.localPosition.z - _startPoint.localPosition.z > 0 ? Color.blue : Color.red;
            _startPoint.localPosition = new Vector3(0, 0, 0);
            _endPoint.localPosition = new Vector3(0, 0, _endPoint.localPosition.z);
            Debug.DrawLine(_startPoint.position, _endPoint.position, drawColor);
        }
    }

    /// <summary>
    /// Get the starting point of the object movement
    /// </summary>
    /// <returns>The starting point of the object movement</returns>
    public Transform GetStartPoint()
    {
        return _startPoint;
    }

    /// <summary>
    /// Get the ending point of the object movement
    /// </summary>
    /// <returns>The ending point of the object movement</returns>
    public Transform GetEndPoint()
    {
        return _endPoint;
    }
}