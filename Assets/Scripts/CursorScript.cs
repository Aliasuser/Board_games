using UnityEngine;

public class CursorScript : MonoBehaviour
{

    public Texture2D[] cursor;


    void Start()
    {
        DefaultCursor();
    }

    public void DefaultCursor()
    {
        Cursor.SetCursor(cursor[0], Vector2.zero, CursorMode.ForceSoftware);
    }

    public void OnButtonCursor()
    {
        Cursor.SetCursor(cursor[1], Vector2.zero, CursorMode.ForceSoftware);
    }

    public void ClickedButtonCursor()
    {
        Cursor.SetCursor(cursor[2], Vector2.zero, CursorMode.ForceSoftware);
    }

    public void OnPropCursor()
    {
        Cursor.SetCursor(cursor[3], Vector2.zero, CursorMode.ForceSoftware);
    }

    public void AttentionCursor()
    {
        Cursor.SetCursor(cursor[4], Vector2.zero, CursorMode.ForceSoftware);
    }
}
