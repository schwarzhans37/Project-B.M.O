using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    //오프라인 씬들 전환용
    public void MenuSceneLoad()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void MakingHostSceneLoad()
    {
        SceneManager.LoadScene("MakingHostScene");
    }

    public void JoinRoomSceneLoad()
    {
        SceneManager.LoadScene("JoinRoomScene");
    }

    public void SettingSceneLoad()
    {
        SceneManager.LoadScene("SettingScene");
    }
    public void LoginSceneLoad()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
