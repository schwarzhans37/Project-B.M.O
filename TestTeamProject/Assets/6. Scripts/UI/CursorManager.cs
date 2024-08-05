using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;
        
    public Texture2D defaultCursorTexture;
    public Texture2D hoverCursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 오브젝트가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
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
