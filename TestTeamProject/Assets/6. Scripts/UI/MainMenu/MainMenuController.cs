using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public MainMenuView mainMenuView;

    void Start()
    {
        mainMenuView.OnCreateHostAttempt += HandleCreateHostAttempt;
        mainMenuView.OnJoinHostAttempt += HandleJoinHostAttempt;
        mainMenuView.OnSettingAttempt += HandleSettingAttempt;
        mainMenuView.OnExitAttempt += HandleExitAttempt;
        mainMenuView.OnWebLinkAttempt += HandleWebLinkAttempt;
    }

    void HandleCreateHostAttempt()
    {

    }

    void HandleJoinHostAttempt()
    {
        
    }

    void HandleSettingAttempt()
    {
        
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
