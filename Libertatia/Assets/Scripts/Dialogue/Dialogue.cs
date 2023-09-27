using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class Dialogue
{
    public string speakerName;

    [TextArea(3, 10)]
    public string[] sentences;

    public UnityEvent callback;
}
