using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class TaskList
{
    public string taskListName;

    [TextArea(3, 10)]
    public string[] tasks;
}
