using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private bool isMoving = false;
    private float aspectRatio;

    //[Range(0.01f, 10.0f)] [SerializeField] // (IS Pet Peave #1) Input system has issues with serializing fields
    private float mouseSensitivity = 1.85f; // Calced to 1920x1080
    private Vector2 previousPos;

    private const float MINIMUM_ZOOM = 1;
    private const float MAXIMUM_ZOOM = 10;
    private float zoomStep = 1.0f;
    private float zoomPercentage;

    private Camera camera;
    private Controls controls;

    private float movementHorizontalScale;
    private float movementVerticalScale;

    private void Awake()
    {
        aspectRatio = (float)Screen.width / Screen.height;
        // Tried to scale sensitivity depending on screen size, but doen't seem to be enough
        movementHorizontalScale = (float)Screen.currentResolution.height/Screen.height;
        movementVerticalScale = (float)Screen.currentResolution.width/Screen.width;

        camera = GetComponent<Camera>();
        camera.orthographicSize = MAXIMUM_ZOOM;
        zoomPercentage = camera.orthographicSize / MAXIMUM_ZOOM;

        controls = new Controls();
        controls.Camera.Move.Enable();
        controls.Camera.Move.performed += Move;
        controls.Camera.Zoom.Enable();
        controls.Camera.Zoom.performed += Zoom;
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (isMoving)
        {
            // 2D solution
            Vector2 currentPos = Mouse.current.position.value;
            Vector2 posDifference = previousPos - currentPos;
            // Calculate movement
            Vector3 horizontalMove = Vector3.right * posDifference.x * zoomPercentage * movementHorizontalScale * mouseSensitivity / 100;
            Vector3 verticalMove = Vector3.forward * posDifference.y * zoomPercentage * movementVerticalScale * mouseSensitivity /100 * aspectRatio;
            // Combine movement
            Vector3 movement = horizontalMove + verticalMove;
            // Update camera position
            transform.position += movement;
            // Store value
            previousPos = currentPos;
        }
    }

    // Input Callback Functions
    private void Move(InputAction.CallbackContext context)
    {
        // Pressed
        if (context.ReadValue<float>() == 1)
        {
            isMoving = true;
            previousPos = Mouse.current.position.value;
        }
        // Release
        else
        {
            isMoving = false;
        }
    }
    private void Zoom(InputAction.CallbackContext context)
    {
        // Get zoom input
        float zoomInput = context.ReadValue<float>();
        float zoomSpeed = zoomInput * zoomStep;

        // Update camera zoom
        camera.orthographicSize += zoomSpeed;

        // Clamp
        if(camera.orthographicSize > MAXIMUM_ZOOM)
        {
            camera.orthographicSize = MAXIMUM_ZOOM;
        }
        else if(camera.orthographicSize < MINIMUM_ZOOM)
        {
            camera.orthographicSize = MINIMUM_ZOOM;
        }
        // Mathf.Clamp(camera.orthographicSize, MINIMUM_ZOOM, MAXIMUM_ZOOM); // why is this not working
        zoomPercentage = camera.orthographicSize / MAXIMUM_ZOOM;
    }
}
