using UnityEngine;
using UnityEngine.UI;

// TODO: use buttons instead of image
public class Tab : MonoBehaviour
{
    private Button button;
    private Image background;
    private Color inactiveColor;
    [SerializeField] private Color activeColor;

    private void Awake()
    {
        background = GetComponent<Image>();
        button = GetComponent<Button>();

        // Maybe move to start
        //background.color = button.colors.normalColor;
        inactiveColor = background.color;
        button.onClick.AddListener(Select);
        Deselect();
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
