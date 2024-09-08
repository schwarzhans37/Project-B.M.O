using UnityEngine;

[RequireComponent(typeof(GameUIView))]
public class GameUIController : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    public GameUIView gameUIView;

    void OnValidate()
    {
        if (gameUIView == null)
            gameUIView = GetComponent<GameUIView>();
    }

    void Start()
    {
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            gameUIView.SetActiveMenuUI();
        }
    }
}
