using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Move calculations to a non-monobehavior class
public class CameraManager : MonoBehaviour
{
    // Components
    private static CameraManager instance;
    private Camera cameraScript; // could get away with Camera.main, but this might be better
    // Mouse
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private Plane dragPlane;
    // Zooming
    [SerializeField] private const float MINIMUM_ZOOM = 4;
    [SerializeField] private const float MAXIMUM_ZOOM = 20;
    [SerializeField] private float zoomSensitivity = 40.0f;
    // Keyboard
    [SerializeField] private float keyboardSensitivity = 50.0f;
    [SerializeField] private float shiftSensitivity = 20.0f;
    // Edge Scrolling
    [SerializeField] private float edgeSize = 30.0f;
    [SerializeField] private float edgeScrollingSensitivity = 30.0f;
    // Bounds
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Vector2 maxWorldBounds; // could be set with size
    [SerializeField] private Vector2 minWorldBounds;

    public static CameraManager Instance
    {
        get
        {
            return instance;
        }
    }
    public Camera Camera
    {
        get
        {
            return cameraScript;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        cameraScript = GetComponent<Camera>();
        cameraScript.orthographicSize = 14;
        dragPlane = new Plane(Vector3.up, Vector3.zero); // flat plane for dragging
    }
    private void Update()
    {
        if(HandleMouseValidation())
        {
            HandleZoom();
            HandleEdgeScrolling();
            HandleKeyboardInput();
            HandleMouseInput();
            if (useBounds)
            {
                CheckBounds();
            }
        }
    }

    private bool HandleMouseValidation()
    {
        // Check if mouse is in bounds
        if (Input.mousePosition.x < 0 ||
            Input.mousePosition.x > Screen.width||
            Input.mousePosition.y < 0 ||
            Input.mousePosition.y > Screen.height)
        {
            return false;
        }

        // Check if mouse is over any UI
        if (EventSystem.current.IsPointerOverGameObject()) // maybe implement IPointerClickHandler
        {
            return false;
        }
        return true;
    }
    private void HandleZoom()
    {
        float orthoSizeDelta = 0.0f;
        if(Input.GetKey(KeyCode.KeypadPlus))
        {
            orthoSizeDelta -= zoomSensitivity * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.KeypadMinus))
        {
            orthoSizeDelta += zoomSensitivity * Time.deltaTime;
        }

        if(Input.mouseScrollDelta.y > 0.0f)
        {
            orthoSizeDelta -= zoomSensitivity * Time.deltaTime;
        }
        if (Input.mouseScrollDelta.y < 0.0f)
        {
            orthoSizeDelta += zoomSensitivity * Time.deltaTime;
        }

        cameraScript.orthographicSize += orthoSizeDelta;
        cameraScript.orthographicSize = Mathf.Clamp(cameraScript.orthographicSize, MINIMUM_ZOOM, MAXIMUM_ZOOM);
    }
    private void HandleEdgeScrolling()
    {
        Vector3 scrollingDelta = Vector3.zero;

        if(Input.mousePosition.x > Screen.width - edgeSize)
        {
            scrollingDelta.x += edgeScrollingSensitivity * Time.deltaTime;

        }
        if (Input.mousePosition.x < edgeSize)
        {
            scrollingDelta.x -= edgeScrollingSensitivity * Time.deltaTime;
        }
        if(Input.mousePosition.y > Screen.height - edgeSize - 64) // 64 is height of resource UI
        {
            scrollingDelta.z += edgeScrollingSensitivity * Time.deltaTime;
        }
        if (Input.mousePosition.y < edgeSize)
        {
            scrollingDelta.z -= edgeScrollingSensitivity * Time.deltaTime;
        }
        transform.position += scrollingDelta;
    }
    private void HandleKeyboardInput()
    {
        float shiftDelta = 0.0f;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            shiftDelta = shiftSensitivity;
        }

        Vector3 keyboardDelta = Vector3.zero;

        float nsDelta = (keyboardSensitivity+ shiftDelta) * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
        {
            keyboardDelta.z += nsDelta;
        }
        if (Input.GetKey(KeyCode.S))
        {
            keyboardDelta.z -= nsDelta;
        }

        float ewDelta = (keyboardSensitivity + shiftDelta) * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            keyboardDelta.x -= ewDelta;
        }
        if (Input.GetKey(KeyCode.D))
        {
            keyboardDelta.x += ewDelta;
        }
        transform.position += keyboardDelta;
    }
    private void HandleMouseInput()
    {
        if(Input.GetMouseButtonDown(2)) // Clicked
        {
            Ray ray = cameraScript.ScreenPointToRay(Input.mousePosition);
            if(dragPlane.Raycast(ray,out float entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(2)) // Holding
        {
            Ray ray = cameraScript.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
                transform.position = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }
    }
    private void CheckBounds()
    {
        Vector3 cameraPos = transform.position;
        if (cameraPos.x > maxWorldBounds.x)
        {
            cameraPos.x = maxWorldBounds.x;
        }
        else if (cameraPos.x < minWorldBounds.x)
        {
            cameraPos.x = minWorldBounds.x;
        }

        if (cameraPos.z > maxWorldBounds.y)
        {
            cameraPos.z = maxWorldBounds.y;
        }
        else if (cameraPos.z < minWorldBounds.y)
        {
            cameraPos.z = minWorldBounds.y;
        }
        transform.position = cameraPos;
    }
}
