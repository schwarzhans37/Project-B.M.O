using UnityEngine;

[RequireComponent(typeof(MainMenuView))]
public class MainMenuController : MonoBehaviour
{
    public MainMenuView mainMenuView;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (mainMenuView == null)
            mainMenuView = GetComponent<MainMenuView>();
    }
#endif

    void Start()
    {
        mainMenuView.OnExitAttempt += HandleExitAttempt;
        mainMenuView.OnWebLinkAttempt += HandleWebLinkAttempt;
    }

    void HandleExitAttempt()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void HandleWebLinkAttempt()
    {
        Application.OpenURL("http://localhost");
    }
}
