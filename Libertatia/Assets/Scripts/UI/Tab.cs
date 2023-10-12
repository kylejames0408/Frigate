using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    private Button button;
    private Image background;
    private Color inactiveColor = new Color(91,91,91);
    private Color activeColor = Color.black;

    private void Awake()
    {
        background = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(Select);
    }

    public void Select()
    {
        background.color = activeColor;
        button.onClick.RemoveListener(Select);
    }
    public void Deselect()
    {
        background.color = inactiveColor;
        button.onClick.RemoveListener(Deselect);
        button.onClick.AddListener(Select);
    }
}
