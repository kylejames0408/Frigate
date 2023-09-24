using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    private bool isMouseMove = false;
    private float aspectRatio;
    private Vector2 maxWorldBounds; // could be set with size
    private Vector2 minWorldBounds;

    //[Range(0.01f, 10.0f)] [SerializeField] // (IS Pet Peave #1) Input system has issues with serializing fields
    private float mouseSensitivity = 1.85f; // Calced to 1920x1080
    private float keyboardSensitivity = 0.2f;
    private Vector2 previousPos;

    private const float MINIMUM_ZOOM = 1;
    private const float MAXIMUM_ZOOM = 10;
    private float zoomStep = 1.0f;
    private float zoomPercentage;

    private Camera cameraScript;
    private Controls controls;

    private float movementHorizontalScale;
    private float movementVerticalScale;

    private void Awake()
    {
        maxWorldBounds = new Vector2(8,-20);
        minWorldBounds = new Vector2(-8,-30);
        aspectRatio = (float)Screen.width / Screen.height;
        // Tried to scale sensitivity depending on screen size, but doen't seem to be enough
        movementHorizontalScale = (float)Screen.currentResolution.height/Screen.height;
        movementVerticalScale = (float)Screen.currentResolution.width/Screen.width;

        cameraScript = GetComponent<Camera>();
        cameraScript.orthographicSize = MAXIMUM_ZOOM;
        zoomPercentage = cameraScript.orthographicSize / MAXIMUM_ZOOM;

        controls = new Controls();
        controls.Camera.MouseMove.Enable();
        controls.Camera.MouseMove.performed += MouseMove;
        controls.Camera.Zoom.Enable();
        controls.Camera.Zoom.performed += Zoom;
    }
    private void Update()
    {
        UpdateMouseMovement();
        CheckBounds();
    }
    private void FixedUpdate()
    {
        UpdateKeyboardMovement();
    }

    // Update Functions
    private void UpdateMouseMovement()
    {
        if (isMouseMove)
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
    private void UpdateKeyboardMovement()
    {
        Vector3 keyboardMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            keyboardMovement += Vector3.forward * keyboardSensitivity * zoomPercentage * aspectRatio;
        }
        if (Input.GetKey(KeyCode.A))
        {
            keyboardMovement += Vector3.left * keyboardSensitivity * zoomPercentage;
        }
        if (Input.GetKey(KeyCode.S))
        {
            keyboardMovement += Vector3.back * keyboardSensitivity * zoomPercentage * aspectRatio;
        }
        if (Input.GetKey(KeyCode.D))
        {
            keyboardMovement += Vector3.right * keyboardSensitivity * zoomPercentage;
        }
        transform.position += keyboardMovement;
    }

    // Input Callback Functions
    private void MouseMove(InputAction.CallbackContext context)
    {
        // Pressed
        if (context.ReadValue<float>() == 1)
        {
            isMouseMove = true;
            previousPos = Mouse.current.position.value;
        }
        // Release
        else
        {
            isMouseMove = false;
        }
    }
    private void Zoom(InputAction.CallbackContext context)
    {
        // Get zoom input
        float zoomInput = context.ReadValue<float>();
        float zoomSpeed = zoomInput * zoomStep;

        // Update camera zoom
        cameraScript.orthographicSize += zoomSpeed;

        // Clamp
        if(cameraScript.orthographicSize > MAXIMUM_ZOOM)
        {
            cameraScript.orthographicSize = MAXIMUM_ZOOM;
        }
        else if(cameraScript.orthographicSize < MINIMUM_ZOOM)
        {
            cameraScript.orthographicSize = MINIMUM_ZOOM;
        }
        // Mathf.Clamp(camera.orthographicSize, MINIMUM_ZOOM, MAXIMUM_ZOOM); // why is this not working
        zoomPercentage = cameraScript.orthographicSize / MAXIMUM_ZOOM;
    }

    // Utility Functions
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
