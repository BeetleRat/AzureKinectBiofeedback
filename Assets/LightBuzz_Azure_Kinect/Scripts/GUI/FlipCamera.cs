using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class FlipCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private FlipMode _flip = FlipMode.None;

    private RectTransform _rect;

    private void Awake()
    {
        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }
        if (_canvas == null)
        {
            _canvas = GetComponent<Canvas>();
        }
    }

    private void Start()
    {
        if (_canvas != null)
        {
            _rect = _canvas.GetComponent<RectTransform>();
        }
    }

    private void OnPreCull()
    {
        float x = _flip == FlipMode.Horizontal ? -1.0f : 1.0f;
        float y = _flip == FlipMode.Vertical ? -1.0f : 1.0f;
        float z = 1.0f;

        _camera.ResetWorldToCameraMatrix();
        _camera.ResetProjectionMatrix();               
        _camera.projectionMatrix = _camera.projectionMatrix * Matrix4x4.Scale(new Vector3(x, y, z));

        if (_rect != null)
        {
            _rect.sizeDelta = new Vector2(Screen.width, Screen.height);
        }
    }

    private void OnPreRender()
    {
        GL.invertCulling = _flip != FlipMode.None;
    }

    private void OnPostRender()
    {
        GL.invertCulling = false;
    }
}

public enum FlipMode
{
    None,
    Horizontal,
    Vertical
}