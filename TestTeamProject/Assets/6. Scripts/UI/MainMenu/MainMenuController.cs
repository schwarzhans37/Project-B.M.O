using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public MainMenuView mainMenuView;

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
