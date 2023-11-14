using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Tooltip("When this script is applied to a canvas object, it will follow the camera, giving the perception of a static position")]
public class PersistentUI : MonoBehaviour
{

    private Camera m_Camera;
    private Vector3 offsetPosition;
    private RectTransform m_RectTransform;
    private Canvas m_Canvas;
    
    [Tooltip("Set this to true if the UI element is above the center of the screen")]
    public bool aboveCam;

    [Tooltip("Set this to true if the UI element is left of the center of the screen")]
    public bool leftCam;

    private void Awake()
    {

        offsetPosition = GetComponent<RectTransform>().anchoredPosition3D;
        m_Canvas = FindObjectOfType<Canvas>();
        m_Camera = FindAnyObjectByType<Camera>();
        m_RectTransform = GetComponent<RectTransform>();
        

        if(m_Canvas.renderMode != RenderMode.WorldSpace)
        {
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX;
        float offsetY;

        if (aboveCam)
            offsetY = offsetPosition.y;
        else
            offsetY = -offsetPosition.y;

        if (!leftCam)
            offsetX = offsetPosition.x;
        else
            offsetX = -offsetPosition.x;
            
        m_RectTransform.position = new Vector3(m_Camera.transform.position.x + offsetX, m_Camera.transform.position.y + offsetY, offsetPosition.z);
    }
}
