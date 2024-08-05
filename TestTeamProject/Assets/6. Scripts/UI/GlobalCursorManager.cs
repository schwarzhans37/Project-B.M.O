using UnityEngine;

public class GlobalCursorManager : MonoBehaviour
{
    public Texture2D defaultCursorTexture;
    public Texture2D hoverCursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트가 파괴되지 않도록 설정
        SetDefaultCursor();
    }

    public void SetCursor(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public void SetDefaultCursor()
    {
        SetCursor(defaultCursorTexture);
    }

    public void SetHoverCursor()
    {
        SetCursor(hoverCursorTexture);
    }
}
