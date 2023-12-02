using UnityEngine;

/// <summary>
/// <para>Script used to move an object using the keyboard</para>
/// </summary>
public class KeyboardObjectMovement : MonoBehaviour
{
    [SerializeField] private Transform _movedObject;
    [SerializeField] private Transform _leftBorder;
    [SerializeField] private Transform _rightBorder;
    [SerializeField] private Transform _forwardBorder;
    [SerializeField] private Transform _backwardBorder;
    [SerializeField] private float _walkingSpeed = 6f;
    [SerializeField] private float _sitPosition = 0.9f;

    private Vector3 playerVelocity;
    private Vector3 maxPosition;
    private Vector3 minPosition;
    private float startYPosition;
    private float currentYPosition;

    private void Start()
    {
        gameObject.SetActive(_movedObject.gameObject.activeInHierarchy);
        SetXMaxMinPosition();
        SetZMaxMinPosition();
        startYPosition = gameObject.transform.localPosition.y;
        currentYPosition = startYPosition;
    }

    private void SetXMaxMinPosition()
    {
        if (!_leftBorder || !_rightBorder)
        {
            maxPosition.x = float.MaxValue;
            minPosition.x = float.MinValue;
        }
        else
        {
            maxPosition.x = Mathf.Max(_leftBorder.position.x, _rightBorder.position.x);
            minPosition.x = Mathf.Min(_leftBorder.position.x, _rightBorder.position.x);
        }
    }

    private void SetZMaxMinPosition()
    {
        if (!_forwardBorder || !_backwardBorder)
        {
            maxPosition.z = float.MaxValue;
            minPosition.z = float.MinValue;
        }
        else
        {
            maxPosition.z = Mathf.Max(_forwardBorder.position.z, _backwardBorder.position.z);
            minPosition.z = Mathf.Min(_forwardBorder.position.z, _backwardBorder.position.z);
        }
    }

    private void Update()
    {
        MoveLeftRight();
        MoveForwardBackward();
        Sit();
    }

    private void MoveLeftRight()
    {
        float horizontalMove = (-1.0f) * Input.GetAxis("Horizontal");
        float move = horizontalMove * _walkingSpeed * Time.deltaTime;
        Vector3 position = _movedObject.position;
        float newXPosition = Mathf.Clamp(position.x + move, minPosition.x, maxPosition.x);
        Vector3 newPosition = new Vector3(newXPosition, position.y, position.z);
        _movedObject.position = newPosition;
    }

    private void MoveForwardBackward()
    {
        float horizontalMove = (-1.0f) * Input.GetAxis("Vertical");
        float move = horizontalMove * _walkingSpeed * Time.deltaTime;
        Vector3 position = _movedObject.position;
        float newZPosition = Mathf.Clamp(position.z + move, minPosition.z, maxPosition.z);
        Vector3 newPosition = new Vector3(position.x, position.y, newZPosition);
        _movedObject.position = newPosition;
    }

    private void Sit()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentYPosition = startYPosition - _sitPosition;
        }
        else
        {
            currentYPosition = startYPosition;
        }

        Vector3 position = _movedObject.position;
        Vector3 newPosition = new Vector3(position.x, currentYPosition, position.z);
        _movedObject.position = newPosition;
    }
}