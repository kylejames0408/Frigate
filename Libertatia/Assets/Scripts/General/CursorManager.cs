using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private static CursorManager instance;
    [SerializeField] private Texture2D iconHandCursor;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    public CursorManager Instance
    { get { return instance; } }

    public void SetCursor(int mode)
    {
        if(mode == 0)
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }
        else if(mode == 1)
        {
            Cursor.SetCursor(iconHandCursor, Vector2.zero, cursorMode);
        }
    }
}
