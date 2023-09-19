using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private float speedMultiplier = 0.1f;
    private float minHeight;
    private float maxHeight;
    
    private float zoomMultiplier = 2.0f;
    private float minZoom;
    private float maxZoom;
    
    private Camera cameraScript;

    private void Awake()
    {
        cameraScript = GetComponent<Camera>();
    }

    private void Update()
    {
        // Get input values
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        float zoomSpeed = Input.GetAxis("Mouse ScrollWheel") * zoomMultiplier; // might need to zoom with projectsion size. Input is inverted

        // Calculate movement
        Vector3 verticalMove = Vector3.forward * verticalInput * speedMultiplier;
        Vector3 horizontalMove = Vector3.right * horizontalInput * speedMultiplier;
        // Combine movement
        Vector3 movement = verticalMove + horizontalMove; // might be good to scale with zoom since the more zoomed in the faster it will seem

        // Update camera zoom
        cameraScript.orthographicSize += zoomSpeed;

        // Update camera position
        transform.position += movement;
    }
}
