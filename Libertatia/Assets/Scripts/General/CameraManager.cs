using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

// TODO: Move calculations to a non-monobehavior class
public class CameraManager : MonoBehaviour
{
    // Components/References
    private static CameraManager instance;
    private Camera cameraScript; // could get away with Camera.main, but this might be better
    // Mouse
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private Plane dragPlane;
    // Zooming
    [SerializeField] private const float MINIMUM_ZOOM = 4;
    [SerializeField] private const float MAXIMUM_ZOOM = 20;
    [SerializeField] private float zoomSensitivity = 50.0f;             // TODO: make option
    // Keyboard
    [SerializeField] private float keyboardSensitivity = 40.0f;         // TODO: make option
    [SerializeField] private float shiftSensitivity = 30.0f;            // TODO: make option
    // Edge Scrolling
    [SerializeField] private bool canEdgeScroll = false;
    [SerializeField] private float edgeSize = 3.0f;                     // TODO: make option
    [SerializeField] private float edgeScrollingSensitivity = 30.0f;    // TODO: make option
    // Bounds
    [SerializeField] private bool usingBounds = false;
    [SerializeField] private Vector2 maxWorldBounds; // could be set with size
    [SerializeField] private Vector2 minWorldBounds;
    // Pan
    [SerializeField] private float panSpeed = 1.0f; // I think we could make our own that is a consistant speed rather than time

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
        canEdgeScroll = false;
    }
    // TODO: separate calculations with time into fixed while others stay in update
    private void FixedUpdate()
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            HandleZoom();
            HandleMouseInput();
        }

        HandleKeyboardInput();
        if (canEdgeScroll)
        {
            HandleEdgeScrolling();
        }
        if (usingBounds)
        {
            CheckBounds();
        }
    }

    private void HandleZoom()
    {
        float orthoSizeDelta = 0.0f;

        // Keyboard +/-
        if(Input.GetKey(KeyCode.KeypadPlus))
        {
            orthoSizeDelta -= zoomSensitivity * Time.deltaTime;
        }
        if(Input.GetKey(KeyCode.KeypadMinus))
        {
            orthoSizeDelta += zoomSensitivity * Time.deltaTime;
        }

        // Mouse scrollwheel
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

        // Right
        if(Input.mousePosition.x > Screen.width - edgeSize)
        {
            scrollingDelta.x += edgeScrollingSensitivity * Time.deltaTime;
        }
        // Left
        if (Input.mousePosition.x < edgeSize)
        {
            scrollingDelta.x -= edgeScrollingSensitivity * Time.deltaTime;
        }
        // Top
        if(Input.mousePosition.y > Screen.height - edgeSize)
        {
            scrollingDelta.z += edgeScrollingSensitivity * Time.deltaTime;
        }
        // Bottom
        if (Input.mousePosition.y < edgeSize)
        {
            scrollingDelta.z -= edgeScrollingSensitivity * Time.deltaTime;
        }
        transform.position += scrollingDelta;
    }
    private void HandleKeyboardInput()
    {
        // Boost - Shift held speeds up movement
        float shiftDelta = 0.0f;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            shiftDelta = shiftSensitivity;
        }

        // Movement - WASD/UDLR
        Vector3 keyboardDelta = Vector3.zero;
        float nsDelta = (keyboardSensitivity+ shiftDelta) * Time.deltaTime;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            keyboardDelta.z += nsDelta;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            keyboardDelta.z -= nsDelta;
        }

        float ewDelta = (keyboardSensitivity + shiftDelta) * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            keyboardDelta.x -= ewDelta;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            keyboardDelta.x += ewDelta;
        }
        transform.position += keyboardDelta;
    }
    private void HandleMouseInput()
    {
        // Click - Start dragging
        if(Input.GetMouseButtonDown(2))
        {
            Ray ray = cameraScript.ScreenPointToRay(Input.mousePosition);
            if(dragPlane.Raycast(ray,out float entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        // Holding - Calculate dragging
        if (Input.GetMouseButton(2))
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

    // Utility
    internal void PanTo(Vector3 lookAtPosition)
    {
        Vector3 cameraPos = transform.position;
        float cameraPitch = transform.rotation.eulerAngles.x * Mathf.Deg2Rad;

        float deltaY = cameraPos.y - lookAtPosition.y;
        float deltaZ = deltaY / Mathf.Tan(cameraPitch);

        transform.DOMove(new Vector3(lookAtPosition.x, cameraPos.y, lookAtPosition.z - deltaZ), panSpeed);
    }
    internal static void SetEdgeScrolling(bool canEdgeScroll)
    {
        instance.canEdgeScroll = canEdgeScroll;
    }
}
