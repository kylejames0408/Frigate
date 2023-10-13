using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostInstructions : MonoBehaviour
{
    private void OnGUI()
    {
        // the rect that is the canvas
        RectTransform canvasRect = GetComponent<RectTransform>();

        // the style used to set the text size and
        GUIStyle GUIBoxStyle = new GUIStyle(GUI.skin.box);
        GUIBoxStyle.fontSize =  (int)(canvasRect.rect.height * 0.023f);
        GUIBoxStyle.alignment = TextAnchor.MiddleLeft;

        //GUI.Box(new Rect(canvasRect.rect.width * 0.03f, canvasRect.rect.height * 0.05f, canvasRect.rect.width * 0.27f, canvasRect.rect.height * 0.24f),
        //    "- Use WASD to move the camera" +
        //    "\n- Use the scroll wheel to zoom in and out" +
        //    "\n- Click on a building button to select a building" +
        //    "\n- Place a building on open ground if it is blue," +
        //    "\n  you can place it there. Once placed, it will" +
        //    "\n  turn green" +
        //    "\n- Left click on a unit to select them" +
        //    "\n- Right click on a building to assign a selected" +
        //    "\n  unit to that building", GUIBoxStyle);

    }
}
