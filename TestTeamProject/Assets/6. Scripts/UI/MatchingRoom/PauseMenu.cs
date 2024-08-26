using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            if(GameIsPaused)
            {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Resume()
    {
        //Pause메뉴 비활성화
        pauseMenu.SetActive(false);

        //Pause메뉴 상태
        GameIsPaused = false;

    }

    public void Pause() {
        //Pause메뉴 활성화
        pauseMenu.SetActive(true);

        //Pause메뉴 상태
        GameIsPaused = true;
        
    }

    public void ToSettingMenu()
    {
        
    }

    public void BackTotheLobby()
    {
        var manager = CustomNetworkRoomManager.singleton;
        if (manager.mode == Mirror.NetworkManagerMode.Host)
        {
            manager.StopHost();
        }
        else if (manager.mode == Mirror.NetworkManagerMode.ClientOnly)
        {
            manager.StopClient();

        }
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
