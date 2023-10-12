using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{


    private void OnGUI()
    {
        // the rect that is the canvas
        RectTransform canvasRect = GetComponent<RectTransform>();

        GUIStyle GUIBoxStyle = new GUIStyle(GUI.skin.box);
        GUIBoxStyle.fontSize = 25;
        GUIBoxStyle.alignment = TextAnchor.MiddleLeft;

        //GUI.Box(new Rect(canvasRect.rect.width * 0.03f, canvasRect.rect.height * 0.05f, canvasRect.rect.width * 0.25f, canvasRect.rect.height * 0.18f), "- Left click to select/drag select units. \n- Right click to move selected units. \n- Automatically attack enemies\n  upon getting close to them." +
        //    "\n- WASD to move Camera. \n- Press E next to the ship (cube) to return.", GUIBoxStyle);

        //GUI.Box(new Rect(40, 30, 250, 100), "Left click to select/drag select units. \nRight click to move selected units. \nAutomatically attack enemies\n upon getting close to them." +
        //    "\nWASD to move Camera. \nPress E next to the ship (cube) to return.");

    }


}
