using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatInstructions : MonoBehaviour
{
    private void OnGUI()
    {
        // the rect that is the canvas
        RectTransform canvasRect = GetComponent<RectTransform>();

        // the style used to set the text size and 
        GUIStyle GUIBoxStyle = new GUIStyle(GUI.skin.box);
        GUIBoxStyle.fontSize = (int)(canvasRect.rect.height * 0.023f);
        GUIBoxStyle.alignment = TextAnchor.MiddleLeft;

        GUI.Box(new Rect(canvasRect.rect.width * 0.03f, canvasRect.rect.height * 0.05f, canvasRect.rect.width * 0.29f, canvasRect.rect.height * 0.22f),
            "- Use WASD to move Camera." +
            "\n- Left click to select a unit." +
            "\n- Drag select to select multiple units at once."+
            "\n- Right click a position to move selected units to it." +
            "\n- Units automatically attack enemies as they get" +
            "\n  close to them." +
            "\n- Move a crewmate to the ship and press E next to" +
            "\n  the ship to return to the outpost.", GUIBoxStyle);

    }
}
