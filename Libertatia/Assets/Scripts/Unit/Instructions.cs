using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instructions : MonoBehaviour
{
    private void OnGUI()
    {
        GUI.Box(new Rect(40, 30, 250, 80), "Left click to select/drag select units. \nRight click to move selected units. \nAutomatically attack enemies\n upon getting close to them.");
    }
}
