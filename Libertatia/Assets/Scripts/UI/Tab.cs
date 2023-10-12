using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    private Button button;
    private Image background;
    private Color inactiveColor = new Color(0.35f, 0.35f, 0.35f, 1.0f);
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
    }
    public void Deselect()
    {
        background.color = inactiveColor;
    }
}
